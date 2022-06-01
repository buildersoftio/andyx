using Buildersoft.Andy.X.Model.App.Messages;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Inbound
{
    public interface IInboundMessageService
    {
        public void AcceptMessage(Message message);
        public void AcceptUnacknowledgedMessage(MessageAcknowledgementFileContent messageAcknowledgement);
    }
}
