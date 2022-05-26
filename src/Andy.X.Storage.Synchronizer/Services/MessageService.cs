using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Utility.Extensions.Json;
using Cortex.Collections.Generic;
using EFCore.BulkExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andy.X.Storage.Synchronizer.Services
{
    public class MessageService
    {
        private readonly LedgerService _ledgerService;
        private readonly StorageService _storageService;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly ConcurrentPriorityQueue<Message, DateTimeOffset> _messageBuffer;
        private readonly ConcurrentQueue<string> binFilesToRemove;

        public MessageService(LedgerService ledgerService, StorageService storageService, StorageConfiguration storageConfiguration)
        {
            _ledgerService = ledgerService;
            _storageService = storageService;
            _storageConfiguration = storageConfiguration;
            _messageBuffer = new ConcurrentPriorityQueue<Message, DateTimeOffset>();
            binFilesToRemove = new ConcurrentQueue<string>();
        }

        public long ReadMessages(string rootStoreTempDirectory, long entriesCount, long currentLedgerId)
        {
            var storeDir = new DirectoryInfo(rootStoreTempDirectory);
            var files = storeDir.GetFiles().ToList().OrderBy(x => x.CreationTimeUtc);
            foreach (var file in files)
            {
                var msgFromBin = MessageIOService.ReadMessage_FromBinFile(file.FullName);
                _messageBuffer.TryEnqueue(new Message()
                {
                    LedgerId = currentLedgerId,
                    MessageId = msgFromBin.Id,
                    Headers = msgFromBin.Headers.ToJson(),

                    Payload = msgFromBin.Payload,

                    SentDate = msgFromBin.SentDate,
                    StoredDate = DateTimeOffset.Now,
                }, msgFromBin.SentDate);

                binFilesToRemove.Enqueue(file.FullName);

                entriesCount++;
                if (entriesCount == _storageConfiguration.LedgerSize)
                    break;
            }

            return entriesCount;
        }

        public bool StoreMessages()
        {
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

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveBinFiles()
        {
            try
            {
                while (binFilesToRemove.TryDequeue(out string fileLocation))
                {
                    File.Delete(fileLocation);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
