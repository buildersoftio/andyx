using Andy.X.Subscription.Synchronizer.Loggers;
using Buildersoft.Andy.X.Core.Contexts.Storages;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.App.Messages;
using Cortex.Collections.Generic;
using EFCore.BulkExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andy.X.Subscription.Synchronizer.Services
{
    public class MessageAcknowledgementService
    {
        private readonly ConcurrentDictionary<string, MessageAcknowledgementContext> _subscriptonContext;
        private readonly ConcurrentDictionary<string, ConcurrentPriorityQueue<MessageAcknowledgementFileContent, DateTimeOffset>> _buffersForSubscription;

        private readonly ConcurrentQueue<string> _binFilesToRemove;

        public MessageAcknowledgementService()
        {
            _subscriptonContext = new ConcurrentDictionary<string, MessageAcknowledgementContext>();
            _buffersForSubscription = new ConcurrentDictionary<string, ConcurrentPriorityQueue<MessageAcknowledgementFileContent, DateTimeOffset>>();
            _binFilesToRemove = new ConcurrentQueue<string>();
        }

        private void TryAllSubscriptionBuffer(string tenant, string product, string component, string topic, string subscription)
        {
            if (_subscriptonContext.ContainsKey(subscription) == true)
                return;

            _subscriptonContext.TryAdd(subscription, new MessageAcknowledgementContext(tenant, product, component, topic, subscription));
            _subscriptonContext[subscription].ChangeTracker.AutoDetectChangesEnabled = false;
            _subscriptonContext[subscription].Database.EnsureCreated();

            _buffersForSubscription.TryAdd(subscription, new ConcurrentPriorityQueue<MessageAcknowledgementFileContent, DateTimeOffset>());
        }

        public long ReadMessages(string tempDirLocation, string searchPattern = "ua_*", long sizeFromDeletedFiles = 0)
        {
            var storeDir = new DirectoryInfo(tempDirLocation);
            var files = storeDir.GetFiles(searchPattern).ToList().OrderBy(x => x.CreationTimeUtc);

            long sizeToReadFiles = -1;
            if (sizeFromDeletedFiles != 0)
                sizeToReadFiles = sizeFromDeletedFiles / 2;

            long fileRead = 0;
            foreach (var file in files)
            {
                fileRead++;
                var msgFromBin = MessageIOService.ReadAckedMessage_FromBinFile(file.FullName);
                TryAllSubscriptionBuffer(msgFromBin.Tenant, msgFromBin.Product, msgFromBin.Component, msgFromBin.Topic, msgFromBin.Subscription);
                _buffersForSubscription[msgFromBin.Subscription].TryEnqueue(msgFromBin, msgFromBin.CreatedDate);

                _binFilesToRemove.Enqueue(file.FullName);

                if (fileRead == sizeToReadFiles)
                    break;
            }

            return _binFilesToRemove.Count;
        }


        public bool StoreMessages()
        {
            if (_buffersForSubscription.Count == 0)
                return false;

            try
            {
                foreach (var buffer in _buffersForSubscription)
                {
                    List<Buildersoft.Andy.X.Model.Entities.Storages.UnacknowledgedMessage> messageAcknowledgementFileContents = new();
                    while (buffer.Value.TryDequeue(out MessageAcknowledgementFileContent messageAcknowledgementFileContent, out DateTimeOffset priorityDate))
                    {
                        messageAcknowledgementFileContents.Add(new Buildersoft.Andy.X.Model.Entities.Storages.UnacknowledgedMessage()
                        {
                            CreatedDate = messageAcknowledgementFileContent.CreatedDate,
                            EntryId = messageAcknowledgementFileContent.EntryId,
                            LedgerId = messageAcknowledgementFileContent.LedgerId,
                            Subscription = messageAcknowledgementFileContent.Subscription
                        });
                    }

                    _subscriptonContext[buffer.Key].BulkInsert(messageAcknowledgementFileContents, config =>
                    {
                        config.CustomDestinationTableName = "UnacknowledgedMessages";
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                return false;
            }
        }

        public bool DeleteMessages()
        {
            if (_buffersForSubscription.Count == 0)
                return false;

            try
            {
                foreach (var buffer in _buffersForSubscription)
                {
                    List<Buildersoft.Andy.X.Model.Entities.Storages.UnacknowledgedMessage> messageAcknowledgementFileContents = new();
                    while (buffer.Value.TryDequeue(out MessageAcknowledgementFileContent messageAcknowledgementFileContent, out DateTimeOffset priorityDate))
                    {
                        messageAcknowledgementFileContents.Add(new Buildersoft.Andy.X.Model.Entities.Storages.UnacknowledgedMessage()
                        {
                            CreatedDate = messageAcknowledgementFileContent.CreatedDate,
                            EntryId = messageAcknowledgementFileContent.EntryId,
                            LedgerId = messageAcknowledgementFileContent.LedgerId,
                            Subscription = messageAcknowledgementFileContent.Subscription
                        });
                    }

                    var fromDb = _subscriptonContext[buffer.Key].UnacknowledgedMessages
                        .Where(x => messageAcknowledgementFileContents.Select(s => s.LedgerId).Contains(x.LedgerId) && messageAcknowledgementFileContents.Select(s => s.EntryId).Contains(x.EntryId)).ToList();

                    _subscriptonContext[buffer.Key].BulkDelete(fromDb, config =>
                    {
                        config.CustomDestinationTableName = "UnacknowledgedMessages";
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error accured, details={ex.Message}");
                return false;
            }
        }

        public bool RemoveBinFiles()
        {
            try
            {
                while (_binFilesToRemove.TryDequeue(out string fileLocation))
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
