using Buildersoft.Andy.X.Core.Abstractions.Hubs.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Producers;
using Buildersoft.Andy.X.Model.Producers.Events;
using Buildersoft.Andy.X.Router.Hubs.Storages;
using Microsoft.AspNetCore.SignalR;
using System;
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

        public Task ConnectConsumerAsync(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        public Task ConnectProducerAsync(Producer producer)
        {
            throw new NotImplementedException();
        }

        public Task CreateComponentAsync(Component component)
        {
            throw new NotImplementedException();
        }

        public Task CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task CreateTenantAsync(Tenant tenant)
        {
            throw new NotImplementedException();
        }

        public Task CreateTopicAsync(Topic topic)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectConsumerAsync(Consumer consumer)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectProducerAsync(Producer producer)
        {
            throw new NotImplementedException();
        }
    }
}
