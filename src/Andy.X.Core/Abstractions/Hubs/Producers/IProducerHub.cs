using Buildersoft.Andy.X.Model.Producers.Events;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Hubs.Producers
{
    public interface IProducerHub
    {
        Task AndyOrderedDisconnect(string message);
        Task ProducerConnected(ProducerConnectedDetails producerConnectedDetails);
        Task ProducerDisconnected(ProducerDisconnectedDetails producerDisconnectedDetails);

        Task MessageStored(object messageStoredDetails);
    }
}
