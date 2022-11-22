using Buildersoft.Andy.X.Core.Abstractions.Services.Inbound;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class MessageEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly IInboundMessageService _inboundMessageService;
        private readonly NodeConfiguration _nodeConfiguration;

        public MessageEventHandler(
            NodeClusterEventService nodeClusterEventService,
            IInboundMessageService inboundMessageService,
            NodeConfiguration nodeConfiguration)
        {
            _nodeClusterEventService = nodeClusterEventService;
            _inboundMessageService = inboundMessageService;
            _nodeConfiguration = nodeConfiguration;
            

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.MessageReceived += NodeClusterEventService_MessageReceived;
        }

        private void NodeClusterEventService_MessageReceived(Model.Entities.Clusters.ClusterChangeLog obj)
        {
            _inboundMessageService.AcceptMessage(new Model.App.Messages.Message()
            {
                Tenant = obj.Tenant,
                Product = obj.Product,
                Component = obj.Component,
                Topic = obj.Topic,
                Headers = obj.Headers,
                Id = obj.Id,
                Payload = obj.Payload,
                SentDate = obj.SentDate

            }, _nodeConfiguration.NodeId);
        }
    }
}
