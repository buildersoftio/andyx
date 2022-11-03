using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services.CoreState;
using Buildersoft.Andy.X.Model.Clusters.Events;
using Buildersoft.Andy.X.Model.Producers;
using System;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers
{
    public class ProducerEventHandler
    {
        private const string PRODUCER_KEY = "ext_";

        private readonly NodeClusterEventService _nodeClusterEventService;
        private readonly IProducerHubRepository _producerHubRepository;
        private readonly IProducerFactory _producerFactory;
        private readonly ICoreService _coreService;

        public ProducerEventHandler(NodeClusterEventService nodeClusterEventService,
            IProducerHubRepository producerHubRepository,
            IProducerFactory producerFactory,
            ICoreService coreService)
        {
            _nodeClusterEventService = nodeClusterEventService;
            _producerHubRepository = producerHubRepository;
            _producerFactory = producerFactory;
            _coreService = coreService;

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _nodeClusterEventService.ProducerConnected += NodeClusterEventService_ProducerConnected;
            _nodeClusterEventService.ProducerDisconnected += NodeClusterEventService_ProducerDisconnected;
            _nodeClusterEventService.ProducerCreated += _nodeClusterEventService_ProducerCreated;
            _nodeClusterEventService.ProducerDeleted += _nodeClusterEventService_ProducerDeleted;
        }

        private void _nodeClusterEventService_ProducerDeleted(ProducerDeletedArgs obj)
        {
            try
            {
                _coreService.DeleteProducer(obj.Tenant,
                    obj.Product,
                    obj.Component,
                    obj.Topic,
                    obj.Producer, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void _nodeClusterEventService_ProducerCreated(ProducerCreatedArgs obj)
        {
            try
            {
                _coreService.CreateProducer(obj.Tenant,
                    obj.Product,
                    obj.Component,
                    obj.Topic,
                    obj.Producer.Name,
                    obj.Producer.Description,
                    obj.Producer.InstanceType, notifyCluster: false);
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_ProducerDisconnected(ProducerDisconnectedArgs obj)
        {
            try
            {
                var id = PRODUCER_KEY + obj.ProducerConnectionId;
                Producer producerToRemove = _producerHubRepository.GetProducerById(id);
                if (producerToRemove != null)
                {
                    _producerHubRepository.RemoveProducer(id);
                }
            }
            catch (Exception)
            {

            }
        }

        private void NodeClusterEventService_ProducerConnected(ProducerConnectedArgs obj)
        {
            try
            {
                var id = PRODUCER_KEY + obj.ProducerConnectionId;
                _producerHubRepository
                    .AddProducer(id,
                        _producerFactory
                            .CreateProducer(obj.Tenant, obj.Product, obj.Component, obj.Topic, obj.Producer));
            }
            catch (Exception)
            {

            }
        }
    }
}
