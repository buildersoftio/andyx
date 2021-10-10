using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Producers;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Storages
{
    public interface IStorageHubService
    {
        Task CreateTenantAsync(Tenant tenant);

        Task CreateProductAsync(string tenant, Product product);
        Task UpdateProductAsync(string tenant, Product product);

        Task CreateComponentAsync(string tenant, string product, Component component);
        Task UpdateComponentAsync(string tenant, string product, Component component);

        Task CreateTopicAsync(string tenant, string product, string component, Topic topic);
        Task UpdateTopicAsync(string tenant, string product, string component, Topic topic);

        Task ConnectProducerAsync(Producer producer);
        Task DisconnectProducerAsync(Producer producer);

        Task ConnectConsumerAsync(Consumer consumer);
        Task DisconnectConsumerAsync(Consumer consumer);
        Task RequestUnacknowledgedMessagesConsumer(Consumer consumer);

        Task StoreMessage(Message message);
        Task AcknowledgeMessage(string tenant, string product, string component, string topic, string consumerName, bool isAcknowledged, Guid messageId);

    }
}
