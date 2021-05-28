using Buildersoft.Andy.X.Core.Abstractions.Hubs.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Producers;
using Buildersoft.Andy.X.Router.Hubs.Storages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Storages
{
    public class StorageHubService : IStorageHubService
    {
        // TODO: implement StorageHubService

        private readonly IHubContext<StorageHub, IStorageHub> hub;
        private readonly IStorageHubRepository storageHubRepository;

        public StorageHubService(IHubContext<StorageHub, IStorageHub> hub, IStorageHubRepository storageHubRepository)
        {
            this.hub = hub;
            this.storageHubRepository = storageHubRepository;
        }

        public async Task ConnectConsumerAsync(Consumer consumer)
        {
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ConsumerConnected(new Model.Consumers.Events.ConsumerConnectedDetails()
                    {
                        Id = consumer.Id,
                        Tenant = consumer.Tenant,
                        Product = consumer.Product,
                        Component = consumer.Component,
                        Topic = consumer.Topic,
                        ConsumerName = consumer.ConsumerName,
                        ConsumerType = consumer.ConsumerType
                    });
                }
            }
        }

        public async Task ConnectProducerAsync(Producer producer)
        {
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProducerConnected(new Model.Producers.Events.ProducerConnectedDetails()
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
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ComponentCreated(new Model.Storages.Events.Components.ComponentCreatedDetails()
                    {
                        Id = component.Id,
                        Name = component.Name,

                        Tenant = tenant,
                        Product = product
                    });
                }
            }
        }

        public async Task CreateProductAsync(string tenant, Product product)
        {
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProductCreated(new Model.Storages.Events.Products.ProductCreatedDetails()
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
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).TenantCreated(new Model.Storages.Events.Tenants.TenantCreatedDetails()
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        DigitalSignature = tenant.DigitalSignature
                    });
                }
            }
        }

        public async Task CreateTopicAsync(string tenant, string product, string component, Topic topic)
        {
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).TopicCreated(new Model.Storages.Events.Topics.TopicCreatedDetails()
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                        Schema = topic.Schema,
                        Tenant = tenant,
                        Product = product,
                        Component = component
                    });
                }
            }
        }

        public async Task DisconnectConsumerAsync(Consumer consumer)
        {
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ConsumerDisconnected(new Model.Consumers.Events.ConsumerDisconnectedDetails()
                    {
                        Id = consumer.Id,
                        Tenant = consumer.Tenant,
                        Product = consumer.Product,
                        Component = consumer.Component,
                        Topic = consumer.Topic,
                        ConsumerName = consumer.ConsumerName
                    });
                }
            }
        }

        public async Task DisconnectProducerAsync(Producer producer)
        {
            foreach (var storage in storageHubRepository.GetStorages())
            {
                int index = new Random().Next(storage.Value.Agents.Count);
                if (!storage.Value.Agents.IsEmpty)
                {
                    await hub.Clients.Client(storage.Value.Agents.Keys.ElementAt(index)).ProducerDisconnected(new Model.Producers.Events.ProducerDisconnectedDetails()
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
    }
}
