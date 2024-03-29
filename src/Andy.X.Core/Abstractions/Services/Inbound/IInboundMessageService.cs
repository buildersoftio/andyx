﻿using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Consumers.Events;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Inbound
{
    public interface IInboundMessageService
    {
        public void AcceptMessage(Message message, string nodeId = "");
        public void AcceptUnacknowledgedMessage(string tenant, string product, string component, string topic, string subscription, MessageAcknowledgedDetails messageAcknowledgement);

        bool TryCreateTopicConnector(string topicKey, int countNodesConnected);
        bool TryCreateClusterAsyncConnector(string nodeId, int countNodesConnected);

        void NodeIsRunningInsideACluster();
    }
}
