using System.IO;

namespace Buildersoft.Andy.X.IO.Locations
{
    public static class TenantLocations
    {
        public static string GetTenantDirectory(string tenant)
        {
            return Path.Combine(ConfigurationLocations.StorageDirectory(), tenant);
        }

        public static string GetProductRootDirectory(string tenantName)
        {
            return Path.Combine(GetTenantDirectory(tenantName));
        }

        public static string GetTenantLogsRootDirectory(string tenantName)
        {
            return Path.Combine(GetTenantDirectory(tenantName), "logs");
        }

        public static string GetTenantTokensDirectory(string tenantName)
        {
            return Path.Combine(GetTenantDirectory(tenantName), "tokens");
        }



        #region PRODUCTS
        public static string GetProductDirectory(string tenantName, string productName)
        {
            return Path.Combine(GetProductRootDirectory(tenantName), productName);
        }
        public static string GetComponentRootDirectory(string tenantName, string productName)
        {
            return Path.Combine(GetProductDirectory(tenantName, productName));
        }
        #endregion


        #region COMPONENT
        public static string GetComponentDirectory(string tenantName, string productName, string componentName)
        {
            return Path.Combine(GetComponentRootDirectory(tenantName, productName), componentName);
        }
        public static string GetComponentTokenDirectory(string tenantName, string productName, string componentName)
        {
            return Path.Combine(GetComponentDirectory(tenantName, productName, componentName), "tokens");
        }
        public static string GetTopicRootDirectory(string tenantName, string productName, string componentName)
        {
            return Path.Combine(GetComponentDirectory(tenantName, productName, componentName));
        }
        #endregion



        #region TOPIC
        public static string GetTopicDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicRootDirectory(tenantName, productName, componentName), topicName);
        }
        public static string GetProducerRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "producers");
        }

        public static string GetSubscriptionRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "subscriptions");
        }

        public static string GetSubscriptionDirectory(string tenantName, string productName, string componentName, string topicName, string subscriptionName)
        {
            return Path.Combine(GetSubscriptionRootDirectory(tenantName, productName, componentName, topicName), subscriptionName);
        }

        public static string GetSubscriptionLogsDirectory(string tenantName, string productName, string componentName, string topicName, string subscriptionName)
        {
            return Path.Combine(GetSubscriptionDirectory(tenantName, productName, componentName, topicName, subscriptionName), "logs");
        }

        public static string GetSubscriptionUnackedDirectory(string tenantName, string productName, string componentName, string topicName, string subscriptionName)
        {
            return Path.Combine(GetSubscriptionDirectory(tenantName, productName, componentName, topicName, subscriptionName), "unacked");
        }

        public static string GetSubscriptionPositionLogFile(string tenantName, string productName, string componentName, string topicName, string subscriptionName)
        {
            return Path.Combine(GetSubscriptionDirectory(tenantName, productName, componentName, topicName, subscriptionName), "position_log.andx");
        }

        public static string GetSubscriptionAcknowledgementLogFile(string tenantName, string productName, string componentName, string topicName, string subscriptionName)
        {
            return Path.Combine(GetSubscriptionDirectory(tenantName, productName, componentName, topicName, subscriptionName), "acknowledgement_log.andx");
        }

        public static string GetConsumerRootDirectory(string tenantName, string productName, string componentName, string topicName, string subscriptionName)
        {
            return Path.Combine(GetSubscriptionDirectory(tenantName, productName, componentName, topicName, subscriptionName), "consumers");
        }

        public static string GetConsumerDirectory(string tenantName, string productName, string componentName, string topicName, string subscriptionName, string consumerName)
        {
            return Path.Combine(GetConsumerRootDirectory(tenantName, productName, componentName, topicName, subscriptionName), consumerName);
        }

        public static string GetConsumerLogsRootDirectory(string tenantName, string productName, string componentName, string topicName, string subscriptionName, string consumerName)
        {
            return Path.Combine(GetConsumerDirectory(tenantName, productName, componentName, topicName, subscriptionName, consumerName), "logs");
        }

        public static string GetTopicLogRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "logs");
        }

        public static string GetMessageRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "messages");
        }

        public static string GetTopicStateFile(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicLogRootDirectory(tenantName, productName, componentName, topicName), $"{topicName.ToLower()}_current_state.andx");
        }

        public static string GetTempTopicRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "temp");
        }
        public static string GetTempMessageToStoreTopicRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTempTopicRootDirectory(tenantName, productName, componentName, topicName), "store");
        }

        public static string GetNextMessageToStoreFile(string tenantName, string productName, string componentName, string topicName, string msgId)
        {
            return Path.Combine(GetTempMessageToStoreTopicRootDirectory(tenantName, productName, componentName, topicName), $"{msgId}.bin");
        }

        public static string GetTempMessageUnAckedTopicRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTempTopicRootDirectory(tenantName, productName, componentName, topicName), "unacked");
        }

        public static string GetNextUnAckedMessageToStoreFile(string tenantName, string productName, string componentName, string topicName, string msgId)
        {
            return Path.Combine(GetTempMessageUnAckedTopicRootDirectory(tenantName, productName, componentName, topicName), $"{msgId}.bin");
        }
        #endregion
    }
}
