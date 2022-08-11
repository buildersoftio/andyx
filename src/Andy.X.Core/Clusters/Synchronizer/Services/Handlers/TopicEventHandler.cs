using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class TopicEventHandler
    {
        private readonly NodeClusterEventService _nodeClusterEventService;
        public TopicEventHandler(NodeClusterEventService nodeClusterEventService)
        {
            _nodeClusterEventService = nodeClusterEventService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.TopicCreated += NodeClusterEventService_TopicCreated;
            _nodeClusterEventService.TopicUpdated += NodeClusterEventService_TopicUpdated;
            _nodeClusterEventService.TopicDeleted += NodeClusterEventService_TopicDeleted;
        }

        private void NodeClusterEventService_TopicUpdated(Buildersoft.Andy.X.Model.Clusters.Events.TopicUpdatedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TopicDeleted(Buildersoft.Andy.X.Model.Clusters.Events.TopicDeletedArgs obj)
        {
            throw new NotImplementedException();
        }

        private void NodeClusterEventService_TopicCreated(Buildersoft.Andy.X.Model.Clusters.Events.TopicCreatedArgs obj)
        {
            throw new NotImplementedException();
        }
    }
}
