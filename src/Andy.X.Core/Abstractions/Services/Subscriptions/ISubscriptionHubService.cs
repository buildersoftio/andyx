using Buildersoft.Andy.X.Model.Entities.Storages;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Subscriptions
{
    public interface ISubscriptionHubService
    {
        public Task TransmitMessage(string tenant, string product, string component, string topic, string subscriptionName, Message message);
    }
}
