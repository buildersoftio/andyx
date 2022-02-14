using Buildersoft.Andy.X.Core.Abstractions.Hubs.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Producers;
using Buildersoft.Andy.X.Model.Storages.Requests.Components;
using Buildersoft.Andy.X.Model.Storages.Requests.Tenants;
using Buildersoft.Andy.X.Router.Hubs.Storages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Storages
{
    public class StorageHubService : IStorageHubService
    {
        private readonly IHubContext<StorageHub, IStorageHub> _hub;
        private readonly IStorageHubRepository _storageHubRepository;

        public StorageHubService(IHubContext<StorageHub, IStorageHub> hub,
            IStorageHubRepository storageHubRepository)
        {
            _hub = hub;
            _storageHubRepository = storageHubRepository;
        }

        public async Task ConnectConsumerAsync(Consumer consumer)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ConsumerConnected(new Model.Consumers.Events.ConsumerConnectedDetails()
                    {
                        Id = consumer.Id,
                        Tenant = consumer.Tenant,
                        Product = consumer.Product,
                        Component = consumer.Component,
                        Topic = consumer.Topic,
                        ConsumerName = consumer.ConsumerName,
                        SubscriptionType = consumer.SubscriptionType
                    });
                }
            }
        }
        public async Task ConnectProducerAsync(Producer producer)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProducerConnected(new Model.Producers.Events.ProducerConnectedDetails()
                    {
                        Id = producer.Id,
                        Tenant = producer.Tenant,
                        Product = producer.Product,
                        Component = producer.Component,
                        Topic = producer.Topic,
                        ProducerName = producer.ProducerName
                    });
                }
            }
        }


        public async Task CreateComponentAsync(string tenant, string product, Component component)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ComponentCreated(new Model.Storages.Events.Components.ComponentCreatedDetails()
                    {
                        Id = component.Id,
                        Name = component.Name,
                        Settings = component.Settings,

                        Tenant = tenant,
                        Product = product
                    });
                }
            }
        }
        public async Task UpdateComponentAsync(string tenant, string product, Component component)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ComponentUpdated(new Model.Storages.Events.Components.ComponentUpdatedDetails()
                    {
                        Id = component.Id,
                        Name = component.Name,
                        Settings = component.Settings,

                        Tenant = tenant,
                        Product = product
                    });
                }
            }
        }


        public async Task CreateProductAsync(string tenant, Product product)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProductCreated(new Model.Storages.Events.Products.ProductCreatedDetails()
                    {
                        Id = product.Id,
                        Name = product.Name,

                        Tenant = tenant
                    });
                }
            }
        }
        public async Task UpdateProductAsync(string tenant, Product product)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProductUpdated(new Model.Storages.Events.Products.ProductUpdatedDetails()
                    {
                        Id = product.Id,
                        Name = product.Name,

                        Tenant = tenant
                    });
                }
            }
        }


        public async Task CreateTenantAsync(Tenant tenant)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).TenantCreated(new Model.Storages.Events.Tenants.TenantCreatedDetails()
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Settings = tenant.Settings
                    });
                }
            }
        }


        public async Task CreateTopicAsync(string tenant, string product, string component, Topic topic)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).TopicCreated(new Model.Storages.Events.Topics.TopicCreatedDetails()
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                        Tenant = tenant,
                        Product = product,
                        Component = component,

                        TopicSettings = topic.TopicSettings
                    });
                }
            }
        }
        public async Task UpdateTopicAsync(string tenant, string product, string component, Topic topic)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).TopicUpdated(new Model.Storages.Events.Topics.TopicUpdatedDetails()
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                        Tenant = tenant,
                        Product = product,
                        Component = component,
                        TopicSettings = topic.TopicSettings
                    });
                }
            }
        }

        public async Task DisconnectConsumerAsync(Consumer consumer)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ConsumerDisconnected(new Model.Consumers.Events.ConsumerDisconnectedDetails()
                    {
                        Id = consumer.Id,
                        Tenant = consumer.Tenant,
                        Product = consumer.Product,
                        Component = consumer.Component,
                        Topic = consumer.Topic,
                        ConsumerName = consumer.ConsumerName,
                        SubscriptionType = consumer.SubscriptionType
                    });
                }
            }
        }
        public async Task DisconnectProducerAsync(Producer producer)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProducerDisconnected(new Model.Producers.Events.ProducerDisconnectedDetails()
                    {
                        Id = producer.Id,
                        Tenant = producer.Tenant,
                        Product = producer.Product,
                        Component = producer.Component,
                        Topic = producer.Topic,
                        ProducerName = producer.ProducerName
                    });
                }
            }
        }
        public async Task StoreMessage(Message message)
        {
            // TODO: Implement Geo-Replication Settings for this cluster.
            // Check if geo-replication is enabled, add geo-replication feature to appsettings.json soo the developer can config from env-variables.
            // WE are simulating that Geo-Replication is off.

            bool IsGeoReplicationActive = false;
            if (IsGeoReplicationActive == true)
            {
                // Geo-replication is on
                foreach (var storage in _storageHubRepository.GetStorages())
                {
                    //if (storage.Value.ActiveAgentIndex >= storage.Value.Agents.Count)
                    //    storage.Value.ActiveAgentIndex = 0;

                    int index = new Random().Next(storage.Value.Agents.Count);

                    if (!storage.Value.Agents.IsEmpty)
                    {
                        await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).MessageStored(new Model.Storages.Events.Messages.MessageStoredDetails()
                        {
                            Id = message.Id,
                            Tenant = message.Tenant,
                            Product = message.Product,
                            Component = message.Component,
                            Topic = message.Topic,
                            ConsumersCurrentTransmitted = message.ConsumersCurrentTransmitted,
                            MessageRaw = message.MessageRaw,
                            Headers = message.Headers,
                            SentDate = message.SentDate
                        });
                    }
                    //storage.Value.ActiveAgentIndex++;
                }
            }
            else
            {
                // Geo-replication is off - storages are shared
                int indexOfStorage = new Random().Next(_storageHubRepository.GetStorages().Count);

                if (!_storageHubRepository.GetStorages().IsEmpty)
                {
                    var storage = _storageHubRepository.GetStorages().ElementAt(indexOfStorage);
                    int index = new Random().Next(storage.Value.Agents.Count);

                    // For Storages this feature will not work for now. Why?- When more than one producer will produce at the same time, the ActiveAgnetIndex will increase
                    // and it will fail to out of bound index.
                    //if (storage.Value.ActiveAgentIndex >= storage.Value.Agents.Count)
                    //    storage.Value.ActiveAgentIndex = 0;

                    if (!storage.Value.Agents.IsEmpty)
                    {
                        await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).MessageStored(new Model.Storages.Events.Messages.MessageStoredDetails()
                        {
                            Id = message.Id,
                            Tenant = message.Tenant,
                            Product = message.Product,
                            Component = message.Component,
                            Topic = message.Topic,
                            ConsumersCurrentTransmitted = message.ConsumersCurrentTransmitted,
                            MessageRaw = message.MessageRaw,
                            Headers = message.Headers,
                            SentDate = message.SentDate,
                        });
                    }
                    //storage.Value.ActiveAgentIndex++;
                }
            }
        }
        public async Task AcknowledgeMessage(string tenant, string product, string component, string topic, string consumerName, bool isAcknowledged, Guid messageId)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).MessageAcknowledged(new Model.Storages.Events.Messages.MessageAcknowledgedDetails()
                    {
                        Tenant = tenant,
                        Product = product,
                        Component = component,
                        Topic = topic,
                        Consumer = consumerName,
                        IsAcknowledged = isAcknowledged,
                        MessageId = messageId
                    });
                }
            }
        }

        public async Task RequestUnacknowledgedMessagesConsumer(Consumer consumer)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await _hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ConsumerUnacknowledgedMessagesRequested(new Model.Consumers.Events.ConsumerConnectedDetails()
                    {
                        Id = consumer.Id,
                        Tenant = consumer.Tenant,
                        Product = consumer.Product,
                        Component = consumer.Component,
                        Topic = consumer.Topic,
                        ConsumerName = consumer.ConsumerName,
                        SubscriptionType = consumer.SubscriptionType,
                        InitialPosition = consumer.ConsumerSettings.InitialPosition
                    });
                }
            }
        }

        public async Task SendCreateComponentTokenStorage(CreateComponentTokenDetails createComponentTokenDetails)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                if (createComponentTokenDetails.StoragesAlreadySent.Contains(storage.Key) != true)
                {
                    createComponentTokenDetails.StoragesAlreadySent.Add(storage.Key);
                    // send to this storage
                    await _hub.Clients.Client(storage.Value.Agents.First().Key).ComponentTokenCreated(new Model.Storages.Events.Components.ComponentTokenCreatedDetails()
                    {
                        Tenant = createComponentTokenDetails.Tenant,
                        Product = createComponentTokenDetails.Product,
                        Component = createComponentTokenDetails.Component,
                        Token = createComponentTokenDetails.Token,

                        StoragesAlreadySent = createComponentTokenDetails.StoragesAlreadySent,
                    });
                }
            }
        }

        public async Task SendCreateTenantTokenStorage(CreateTenantTokenDetails createTenantTokenDetails)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                if (createTenantTokenDetails.StoragesAlreadySent.Contains(storage.Key) != true)
                {
                    createTenantTokenDetails.StoragesAlreadySent.Add(storage.Key);
                    // send to this storage
                    await _hub.Clients.Client(storage.Value.Agents.First().Key).TenantTokenCreated(new Model.Storages.Events.Tenants.TenantTokenCreatedDetails()
                    {
                        Tenant = createTenantTokenDetails.Tenant,
                        Token = createTenantTokenDetails.Token,
                        StoragesAlreadySent = createTenantTokenDetails.StoragesAlreadySent,
                    });
                }
            }
        }

        public async Task SendRevokeComponentTokenStorage(RevokeComponentTokenDetails revokeComponentTokenDetails)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                if (revokeComponentTokenDetails.StoragesAlreadySent.Contains(storage.Key) != true)
                {
                    revokeComponentTokenDetails.StoragesAlreadySent.Add(storage.Key);
                    // send to this storage
                    await _hub.Clients.Client(storage.Value.Agents.First().Key).ComponentTokenRevoked(new Model.Storages.Events.Components.ComponentTokenRevokedDetails()
                    {
                        Tenant = revokeComponentTokenDetails.Tenant,
                        Token = revokeComponentTokenDetails.Token,
                        StoragesAlreadySent = revokeComponentTokenDetails.StoragesAlreadySent,
                    });
                }
            }
        }

        public async Task SendRevokeTenantTokenStorage(RevokeTenantTokenDetails revokeTenantTokenDetails)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                if (revokeTenantTokenDetails.StoragesAlreadySent.Contains(storage.Key) != true)
                {
                    revokeTenantTokenDetails.StoragesAlreadySent.Add(storage.Key);
                    // send to this storage
                    await _hub.Clients.Client(storage.Value.Agents.First().Key).TenantTokenRevoked(new Model.Storages.Events.Tenants.TenantTokenRevokedDetails()
                    {
                        Tenant = revokeTenantTokenDetails.Tenant,
                        Token = revokeTenantTokenDetails.Token,
                        StoragesAlreadySent = revokeTenantTokenDetails.StoragesAlreadySent,
                    });
                }
            }
        }

        public async Task SendCreateTenantStorage(CreateTenantDetails createTenantDetails)
        {
            foreach (var storage in _storageHubRepository.GetStorages())
            {
                if (createTenantDetails.StoragesAlreadySent.Contains(storage.Key) != true)
                {
                    createTenantDetails.StoragesAlreadySent.Add(storage.Key);
                    // send to this storage
                    await _hub.Clients.Client(storage.Value.Agents.First().Key).TenantCreated(new Model.Storages.Events.Tenants.TenantCreatedDetails()
                    {
                        Id = Guid.NewGuid(),
                        Name = createTenantDetails.Name,
                        Settings = createTenantDetails.TenantSettings,
                        StoragesAlreadySent = createTenantDetails.StoragesAlreadySent,
                    });
                }
            }
        }
    }
}
