using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Consumers;
using Buildersoft.Andy.X.Model.Consumers.Events;
using Buildersoft.Andy.X.Model.Producers;
using Buildersoft.Andy.X.Model.Producers.Events;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Storages
{
    public interface IStorageHubService
    {
        Task CreateTenantAsync(Tenant tenant);
        Task CreateProductAsync(Product product);
        Task CreateComponentAsync(Component component);
        Task CreateTopicAsync(Topic topic);

        Task ConnectProducerAsync(Producer producer);
        Task DisconnectProducerAsync(Producer producer);

        Task ConnectConsumerAsync(Consumer consumer);
        Task DisconnectConsumerAsync(Consumer consumer);
    }
}
