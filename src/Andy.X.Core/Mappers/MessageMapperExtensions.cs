using Buildersoft.Andy.X.Model.App.Topics;
using System;

namespace Buildersoft.Andy.X.Core.Mappers
{
    public static class MessageMapperExtensions
    {
        public static Model.Entities.Storages.Message Map(this Model.App.Messages.Message message, string nodeId, long entry)
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

        public static Model.Entities.Clusters.ClusterChangeLog Map(this Model.App.Messages.Message message, long entry)
        {
            return new Model.Entities.Clusters.ClusterChangeLog()
            {
                Entry = entry,
                Tenant = message.Tenant,
                Product = message.Product,
                Component = message.Component,
                Topic = message.Topic,
                Id = message.Id,
                Headers = message.Headers,
                Payload = message.Payload,
                SentDate = message.SentDate
            };
        }
    }
}
