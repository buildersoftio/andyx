using Buildersoft.Andy.X.Model.App.Messages;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Consumers
{
    public interface IConsumerHubService
    {
        Task TransmitMessage(Message message);
    }
}
