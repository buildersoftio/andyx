using System;

namespace Buildersoft.Andy.X.Core.Mappers
{
    public static class MessageMapperExtensions
    {
        public static Model.Entities.Storages.Message Map(this Model.App.Messages.Message message, string nodeId,long entry)
        {
            return new Model.Entities.Storages.Message()
            {
                Entry = entry,
                Headers = message.Headers,
                MessageId = message.Id,
                Payload = message.Payload,
                NodeId = nodeId,
                SentDate = message.SentDate,
                StoredDate = DateTimeOffset.Now
            };
        }
    }
}
