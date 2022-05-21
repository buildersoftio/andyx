using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Core.Services.Inbound.Connectors;
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

        private readonly ConcurrentDictionary<string, MessageTopicConnector> _topicConnectors;

        public InboundMessageService(ILogger<InboundMessageService> logger, ThreadsConfiguration threadsConfiguration)
        {
            _logger = logger;
            _threadsConfiguration = threadsConfiguration;
            _topicConnectors = new ConcurrentDictionary<string, MessageTopicConnector>();
        }

        public void AcceptMessage(Message message)
        {
            string connectorKey = ConnectorHelper.GetTopicConnectorKey(message.Tenant, message.Product, message.Component, message.Topic);
            TryCreateTopicConnector(connectorKey);

            _topicConnectors[connectorKey].MessagesBuffer.Enqueue(message);

            InitializeInboundMessageProcessor(connectorKey);
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

        private void InboundMessagingProcessor(string connectorKey, Guid key)
        {
            while (_topicConnectors[connectorKey].MessagesBuffer.TryDequeue(out Message message))
            {
                try
                {
                    // store the message.
                }
                catch (Exception)
                {

                    throw;
                }
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
