using Buildersoft.Andy.X.Core.Abstractions.Factories.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Clusters.Synchronizer.Providers;
using Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services.Handlers;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Clusters.Events;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Services
{
    public class NodeClusterEventService
    {
        private readonly ILogger<NodeClusterEventService> _logger;
        private readonly Replica _replica;
        private readonly ClusterConfiguration _clusterConfiguration;

        private readonly IProducerHubRepository _producerHubRepository;
        private readonly IProducerFactory _producerFactory;
        private readonly ISubscriptionHubRepository _subscriptionHubRepository;
        private readonly IConsumerFactory _consumerFactory;
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly ITenantService _tenantService;
        private readonly ITenantFactory _tenantFactory;
        private readonly NodeConfiguration _nodeConfiguration;
        private HubConnection _connection;

        public event Action<NodeConnectedArgs>? NodeConnected;
        public event Action<NodeDisconnectedArgs>? NodeDisconnected;

        // tenants
        public event Action<TenantCreatedArgs>? TenantCreated;
        public event Action<TenantUpdatedArgs>? TenantUpdated;
        public event Action<TenantDeletedArgs>? TenantDeleted;

        public event Action<ProductCreatedArgs>? ProductCreated;
        public event Action<ProductUpdatedArgs>? ProductUpdated;
        public event Action<ProductDeletedArgs>? ProductDeleted;

        public event Action<ComponentCreatedArgs>? ComponentCreated;
        public event Action<ComponentUpdatedArgs>? ComponentUpdated;
        public event Action<ComponentDeletedArgs>? ComponentDeleted;

        public event Action<TopicCreatedArgs>? TopicCreated;
        public event Action<TopicUpdatedArgs>? TopicUpdated;
        public event Action<TopicDeletedArgs>? TopicDeleted;

        public event Action<TokenCreatedArgs>? TokenCreated;
        public event Action<TokenRevokedArgs>? TokenRevoked;

        public event Action<SubscriptionCreatedArgs>? SubscriptionCreated;
        public event Action<SubscriptionUpdatedArgs>? SubscriptionUpdated;
        public event Action<SubscriptionDeletedArgs>? SubscriptionDeleted;

        // only with replicas

        public event Action<SubscriptionPositionUpdatedArgs>? SubscriptionPositionUpdated;
        public event Action<CurrentEntryPositionUpdatedArgs>? CurrentEntryPositionUpdated;

        // will all nodes
        public event Action<ProducerConnectedArgs>? ProducerConnected;
        public event Action<ProducerDisconnectedArgs>? ProducerDisconnected;

        public event Action<ConsumerConnectedArgs>? ConsumerConnected;
        public event Action<ConsumerDisconnectedArgs>? ConsumerDisconnected;

        private NodeEventHandler? nodeEventHandler;
        private TenantEventHandler? tenantEventHandler;
        private ProductEventHandler? productEventHandler;
        private ComponentEventHandler? componentEventHandler;
        private TopicEventHandler? topicEventHandler;
        private TokenEventHandler? tokenEventHandler;
        private SubscriptionEventHandler? subscriptionEventHandler;
        private ProducerEventHandler? producerEventHandler;
        private ConsumerEventHandler? consumerEventHandler;

        public NodeClusterEventService(ILogger<NodeClusterEventService> logger, Replica replica,
            ClusterConfiguration clusterConfiguration,
            IProducerHubRepository producerHubRepository,
            IProducerFactory producerFactory,
            ISubscriptionHubRepository subscriptionHubRepository,
            ITenantService tenantService,
            ITenantFactory tenantFactory,
            NodeConfiguration nodeConfiguration)
        {
            _logger = logger;
            _replica = replica;
            _clusterConfiguration = clusterConfiguration;

            _producerHubRepository = producerHubRepository;
            _producerFactory = producerFactory;
            _subscriptionHubRepository = subscriptionHubRepository;

            _tenantService = tenantService;
            _tenantFactory = tenantFactory;
            _nodeConfiguration = nodeConfiguration;

            var provider = new NodeConnectionProvider(replica, clusterConfiguration, nodeConfiguration);
            _connection = provider.GetHubConnection();

            _connection.Closed += Connection_Closed;
            _connection.Reconnected += Connection_Reconnected;
            _connection.Reconnecting += Connection_Reconnecting;

            // add events here.
            _connection.On<NodeConnectedArgs>("NodeConnectedAsync", connArgs => NodeConnected?.Invoke(connArgs));
            _connection.On<NodeDisconnectedArgs>("NodeDisconnectedAsync", disconnArgs => NodeDisconnected?.Invoke(disconnArgs));

            _connection.On<TenantCreatedArgs>("TenantCreatedAsync", args => TenantCreated?.Invoke(args));
            _connection.On<TenantUpdatedArgs>("TenantUpdatedAsync", args => TenantUpdated?.Invoke(args));
            _connection.On<TenantDeletedArgs>("TenantDeletedAsync", args => TenantDeleted?.Invoke(args));

            _connection.On<ProductCreatedArgs>("ProductCreatedAsync", args => ProductCreated?.Invoke(args));
            _connection.On<ProductUpdatedArgs>("ProductUpdatedAsync", args => ProductUpdated?.Invoke(args));
            _connection.On<ProductDeletedArgs>("ProductDeletedAsync", args => ProductDeleted?.Invoke(args));

            _connection.On<ComponentCreatedArgs>("ComponentCreatedAsync", args => ComponentCreated?.Invoke(args));
            _connection.On<ComponentUpdatedArgs>("ComponentUpdatedAsync", args => ComponentUpdated?.Invoke(args));
            _connection.On<ComponentDeletedArgs>("ComponentDeletedAsync", args => ComponentDeleted?.Invoke(args));

            _connection.On<TopicCreatedArgs>("TopicCreatedAsync", args => TopicCreated?.Invoke(args));
            _connection.On<TopicUpdatedArgs>("TopicUpdatedAsync", args => TopicUpdated?.Invoke(args));
            _connection.On<TopicDeletedArgs>("TopicDeletedAsync", args => TopicDeleted?.Invoke(args));

            _connection.On<TokenCreatedArgs>("TokenCreatedAsync", args => TokenCreated?.Invoke(args));
            _connection.On<TokenRevokedArgs>("TokenRevokedAsync", args => TokenRevoked?.Invoke(args));

            _connection.On<SubscriptionCreatedArgs>("SubscriptionCreatedAsync", args => SubscriptionCreated?.Invoke(args));
            _connection.On<SubscriptionUpdatedArgs>("SubscriptionUpdatedAsync", args => SubscriptionUpdated?.Invoke(args));
            _connection.On<SubscriptionDeletedArgs>("SubscriptionDeletedAsync", args => SubscriptionDeleted?.Invoke(args));


            _connection.On<SubscriptionPositionUpdatedArgs>("SubscriptionPositionUpdatedAsync", args => SubscriptionPositionUpdated?.Invoke(args));
            _connection.On<CurrentEntryPositionUpdatedArgs>("CurrentEntryPositionUpdatedAsync", args => CurrentEntryPositionUpdated?.Invoke(args));

            _connection.On<ProducerConnectedArgs>("ProducerConnectedAsync", args => ProducerConnected?.Invoke(args));
            _connection.On<ProducerDisconnectedArgs>("ProducerDisconnectedAsync", args => ProducerDisconnected?.Invoke(args));

            _connection.On<ConsumerConnectedArgs>("ConsumerConnectedAsync", args => ConsumerConnected?.Invoke(args));
            _connection.On<ConsumerDisconnectedArgs>("ConsumerDisconnectedAsync", args => ConsumerDisconnected?.Invoke(args));


            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            nodeEventHandler = new NodeEventHandler(this);
            tenantEventHandler = new TenantEventHandler(this, _tenantService, _tenantFactory);
            productEventHandler = new ProductEventHandler(this, _tenantService, _tenantFactory);
            componentEventHandler = new ComponentEventHandler(this, _tenantService, _tenantFactory);
            topicEventHandler = new TopicEventHandler(this, _tenantService, _tenantFactory);
            tokenEventHandler = new TokenEventHandler(this);
            subscriptionEventHandler = new SubscriptionEventHandler(this);
            producerEventHandler = new ProducerEventHandler(this, _producerHubRepository, _producerFactory);
            consumerEventHandler = new ConsumerEventHandler(this, _subscriptionHubRepository, _subscriptionFactory, _consumerFactory);
        }


        #region connection general events
        private Task Connection_Reconnecting(Exception? arg)
        {
            _logger.LogWarning($"Node '{_replica.NodeId}' Sync connection is lost, reconnecting");
            return Task.CompletedTask;
        }

        private Task Connection_Reconnected(string? arg)
        {
            _logger.LogInformation($"Node '{_replica.NodeId}' Sync is connected, synchronizing with other nodes.");
            return Task.CompletedTask;
        }

        private Task Connection_Closed(Exception? arg)
        {
            // try to reconnect, if the connection is closed.
            // TODO: add logic to try 5 times, if not destroy the cluster if is the only main in the cluster.
            // or try to move the logic into Node itself. hmmm let see.
            _logger.LogError($"Node '{_replica.NodeId}' Sync is closed, trying to connect");
            ConnectAsync();
            return Task.CompletedTask;
        }
        #endregion


        public async void ConnectAsync()
        {
            await _connection.StartAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    // Details is not show for now...
                    _logger.LogError($"Error occurred during connection to '{_replica.NodeId}' node. Details: {task.Exception.Message}");
                    // retry connection
                    Thread.Sleep(2000);
                    ConnectAsync();
                }
            });
        }
    }
}
