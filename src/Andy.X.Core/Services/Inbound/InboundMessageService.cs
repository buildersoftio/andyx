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
using System.Diagnostics;
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
        }

        public void AcceptUnacknowledgedMessage(MessageAcknowledgementFileContent messageAcknowledgement)
        {
            string connectorKey = ConnectorHelper.GetTopicConnectorKey(messageAcknowledgement.Tenant, messageAcknowledgement.Product, messageAcknowledgement.Component, messageAcknowledgement.Topic);
            TryCreateTopicConnector(connectorKey);

            _topicConnectors[connectorKey].UnacknowledgedMessageBuffer.Enqueue(messageAcknowledgement);

            InitializeInboundUnacknowledgedMessageProcessor(connectorKey);

            // try to run storage service to store records.
            _orchestratorService.StartSubscriptionSynchronizerProcess(connectorKey);
        }

        #region Store Message Region
        private void InitializeInboundMessageProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].MessageStoreThreadingPool.AreThreadsRunning != true)
            {
                _topicConnectors[connectorKey].MessageStoreThreadingPool.AreThreadsRunning = true;

                // initialize threads.
                InitializeInboundMessageProcessorThreads(connectorKey);
            }
        }

        private void InitializeInboundMessageProcessorThreads(string connectorKey)
        {
            foreach (var thread in _topicConnectors[connectorKey].MessageStoreThreadingPool.Threads)
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int k = 0;
            while (_topicConnectors[connectorKey].MessagesBuffer.TryDequeue(out Message message))
            {
                try
                {
                    k++;
                    var msgId = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss_") + Guid.NewGuid();
                    MessageIOService.TrySaveInTemp_MessageBinFile(message, msgId);

                    // TODO: Implement Cluster Syncronization of messages.
                }
                catch (Exception)
                {
                    // TODO: check this later;
                }
            }
            stopwatch.Stop();
            _topicConnectors[connectorKey].MessageStoreThreadingPool.Threads[threadId].IsThreadWorking = false;

            _logger.LogInformation($"thread inbount processed threadId={threadId}; count={k}, time from start {stopwatch.Elapsed.TotalSeconds} s");
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
        #endregion

        #region Store Unacked Messages Region
        private void InitializeInboundUnacknowledgedMessageProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.AreThreadsRunning != true)
            {
                _topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.AreThreadsRunning = true;

                // initialize threads.
                InitializeInboundUnacknowledgedMessageProcessorThreads(connectorKey);
            }
        }
        private void InitializeInboundUnacknowledgedMessageProcessorThreads(string connectorKey)
        {
            foreach (var thread in _topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.Threads)
            {
                if (thread.Value.IsThreadWorking != true)
                {
                    try
                    {
                        thread.Value.IsThreadWorking = true;
                        thread.Value.Task = Task.Run(() => InboundUnacknowledgedMessagingProcessor(connectorKey, thread.Key));
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Inbound unacknowledged message processor thread '{thread.Key}' failed to restart");
                    }
                }
            }
        }

        private void InboundUnacknowledgedMessagingProcessor(string connectorKey, Guid threadId)
        {
            while (_topicConnectors[connectorKey].UnacknowledgedMessageBuffer.TryDequeue(out MessageAcknowledgementFileContent messageAcknowledgement))
            {
                try
                {
                    var msgId = $"ua_{messageAcknowledgement.Subscription}" + Guid.NewGuid();
                    if (messageAcknowledgement.IsDeleted == true)
                        msgId = $"del_" + Guid.NewGuid();

                    MessageIOService.TrySaveInTemp_UnackedMessageBinFile(messageAcknowledgement, msgId);


                    // TODO: Implement Cluster Syncronization of messages.
                }
                catch (Exception)
                {
                    // TODO: check this later;
                }
            }

            _topicConnectors[connectorKey].UnacknowledgedMessageThreadingPool.Threads[threadId].IsThreadWorking = false;

            // check release memory.
            ReleaseMemoryUnacknowledgedMessagingProcessor(connectorKey);
        }

        private void ReleaseMemoryUnacknowledgedMessagingProcessor(string connectorKey)
        {
            if (_topicConnectors[connectorKey].UnacknowledgedMessageBuffer.Count == 0)
            {
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }

        #endregion
    }
}
