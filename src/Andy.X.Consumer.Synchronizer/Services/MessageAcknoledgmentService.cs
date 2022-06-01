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

namespace Andy.X.Consumer.Synchronizer.Services
{
    public class MessageAcknoledgmentService
    {
        private readonly ConcurrentDictionary<string, MessageAcknowledgementContext> _subscriptonContext;
        private readonly ConcurrentDictionary<string, ConcurrentPriorityQueue<MessageAcknowledgementFileContent, DateTimeOffset>> _buffersForSubscription;

        private readonly ConcurrentQueue<string> _binFilesToRemove;

        public MessageAcknoledgmentService()
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

        public void ReadMessages(string tempDirLocation)
        {
            var storeDir = new DirectoryInfo(tempDirLocation);
            var files = storeDir.GetFiles().ToList().OrderBy(x => x.CreationTimeUtc);
            foreach (var file in files)
            {
                var msgFromBin = MessageIOService.ReadAckedMessage_FromBinFile(file.FullName);
                TryAllSubscriptionBuffer(msgFromBin.Tenant, msgFromBin.Product, msgFromBin.Component, msgFromBin.Topic, msgFromBin.Subscription);
                _buffersForSubscription[msgFromBin.Subscription].TryEnqueue(msgFromBin, msgFromBin.CreatedDate);

                _binFilesToRemove.Enqueue(file.FullName);
            }
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
