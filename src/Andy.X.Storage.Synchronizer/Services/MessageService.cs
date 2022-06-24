using Andy.X.Storage.Synchronizer.Loggers;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using Cortex.Collections.Generic;
using EFCore.BulkExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Andy.X.Storage.Synchronizer.Services
{
    public class MessageService
    {
        private readonly LedgerService _ledgerService;
        private readonly StorageService _storageService;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly ConcurrentPriorityQueue<Message, DateTimeOffset> _messageBuffer;
        private readonly List<string> _binFilesToRemove;

        public MessageService(LedgerService ledgerService, StorageService storageService, StorageConfiguration storageConfiguration)
        {
            _ledgerService = ledgerService;
            _storageService = storageService;
            _storageConfiguration = storageConfiguration;

            _messageBuffer = new ConcurrentPriorityQueue<Message, DateTimeOffset>();
            _binFilesToRemove = new List<string>();
        }

        public long ReadMessages(string rootStoreTempDirectory, long entriesCount, long currentLedgerId)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var storeDir = new DirectoryInfo(rootStoreTempDirectory);
            var files = storeDir.GetFiles().ToList().OrderBy(x => x.CreationTimeUtc);
            int batchCount = 0;

            foreach (var file in files)
            {

                var msgFromBin = MessageIOService.ReadMessage_FromBinFile(file.FullName);
                if (msgFromBin != null)
                {
                    _messageBuffer.TryEnqueue(new Message()
                    {
                        LedgerId = currentLedgerId,
                        MessageId = msgFromBin.Id,
                        Headers = msgFromBin.Headers.ToJson(),

                        Payload = msgFromBin.Payload,

                        SentDate = msgFromBin.SentDate,
                        StoredDate = DateTimeOffset.Now,
                    }, msgFromBin.SentDate);


                    //TODO: if the message file will be empty 'DID we lose a message here?!' Try to fix this issue.
                }

                _binFilesToRemove.Add(file.FullName);

                batchCount++;
                entriesCount++;

                if (batchCount == _storageConfiguration.BatchSize)
                    break;

                if (entriesCount == _storageConfiguration.LedgerSize)
                    break;
            }

            stopwatch.Stop();
            Logger.Log($"Messages read from disk for {stopwatch.Elapsed.TotalMilliseconds}");

            return entriesCount;
        }

        public bool StoreMessages()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (_messageBuffer.Count == 0)
                return false;

            try
            {
                List<Message> messagesToStore = new();
                while (_messageBuffer.TryDequeue(out Message message, out DateTimeOffset priority))
                {
                    messagesToStore.Add(message);
                }
                _storageService.GetStorageContext().BulkInsert(messagesToStore);
                stopwatch.Stop();
                Logger.Log($"Messages stored for {stopwatch.Elapsed.TotalMilliseconds}");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveBinFiles()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                Parallel.ForEach(_binFilesToRemove, new ParallelOptions { MaxDegreeOfParallelism = 4 }, file =>
                {
                    File.Delete(file);
                });

                stopwatch.Stop();
                Logger.Log($"Files deleted for {stopwatch.Elapsed.TotalMilliseconds}");
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
