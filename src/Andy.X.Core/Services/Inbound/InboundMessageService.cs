using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Abstractions.Services.Outbound;
using Buildersoft.Andy.X.Core.Services.Inbound.Connectors;
using Buildersoft.Andy.X.IO.Services;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Services.Inbound
{
    public class InboundMessageService : IInboundMessageService
    {
        private readonly ILogger<InboundMessageService> _logger;
        private readonly ThreadsConfiguration _threadsConfiguration;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ConcurrentDictionary<string, MessageTopicConnector> _topicConnectors;
        private readonly IOutboundMessageService _outboundMessageService;

        public InboundMessageService(ILogger<InboundMessageService> logger, 
            ThreadsConfiguration threadsConfiguration, 
            IOrchestratorService orchestratorService,
            IOutboundMessageService outboundMessageService)
        {
            _logger = logger;
            _threadsConfiguration = threadsConfiguration;
            _orchestratorService = orchestratorService;
            _outboundMessageService = outboundMessageService;

            _topicConnectors = new ConcurrentDictionary<string, MessageTopicConnector>();
        }

        public void AcceptMessage(Message message)
        {
            string connectorKey = ConnectorHelper.GetTopicConnectorKey(message.Tenant, message.Product, message.Component, message.Topic);
            TryCreateTopicConnector(connectorKey);

            _topicConnectors[connectorKey].MessagesBuffer.Enqueue(message);

            InitializeInboundMessageProcessor(connectorKey);

            // try to run storage service to store records.
            _orchestratorService.StartTopicStorageSynchronizerProcess(connectorKey);

            _outboundMessageService.TriggerSubscriptionsByProducer(message.Tenant, message.Product, message.Component, message.Topic);
        }

        private void InitializeInboundMessageProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].ThreadingPool.AreThreadsRunning != true)
            {
                _topicConnectors[connectorKey].ThreadingPool.AreThreadsRunning = true;

                // initialize threads.
                InitializeInboundMessageProcessorThreads(connectorKey);
            }
        }

        private void InitializeInboundMessageProcessorThreads(string connectorKey)
        {
            foreach (var thread in _topicConnectors[connectorKey].ThreadingPool.Threads)
            {
                if (thread.Value.IsThreadWorking != true)
                {
                    try
                    {
                        thread.Value.IsThreadWorking = true;
                        thread.Value.Task = Task.Run(() => InboundMessagingProcessor(connectorKey, thread.Key));
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Inbound message processor thread '{thread.Key}' failed to restart");
                    }
                }
            }
        }

        private void InboundMessagingProcessor(string connectorKey, Guid threadId)
        {
            while (_topicConnectors[connectorKey].MessagesBuffer.TryDequeue(out Message message))
            {
                try
                {
                    var msgId = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss_") + Guid.NewGuid();
                    MessageIOService.TrySaveInTemp_MessageBinFile(message, msgId);


                    // TODO: Implement Cluster Syncronization of messages.
                }
                catch (Exception)
                {
                    // TODO: check this later;
                }
            }

            _topicConnectors[connectorKey].ThreadingPool.Threads[threadId].IsThreadWorking = false;

            // check release memory.
            ReleaseMemoryMessagingProcessor(connectorKey);
        }

        private void ReleaseMemoryMessagingProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].MessagesBuffer.Count == 0)
            {
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }

        private bool TryCreateTopicConnector(string connectorKey)
        {
            if (_topicConnectors.ContainsKey(connectorKey))
                return true;

            return _topicConnectors.TryAdd(connectorKey, new MessageTopicConnector(_threadsConfiguration.MaxNumber));
        }
    }
}
