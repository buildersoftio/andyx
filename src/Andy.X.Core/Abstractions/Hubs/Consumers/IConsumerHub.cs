using Buildersoft.Andy.X.Model.Consumers.Events;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Hubs.Consumers
{
    public interface IConsumerHub
    {
        Task ConsumerConnected(ConsumerConnectedDetails consumerConnectedDetails);
        Task ConsumerDisconnected(ConsumerDisconnectedDetails consumerDisconnectedDetails);

        Task MessageSent(object messageSentDetails);
        Task MessageAcknowledged(object messageAcknowledgedDetails);
    }
}
