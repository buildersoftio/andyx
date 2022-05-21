﻿using System.IO;

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

        public static string GetConsumerRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "consumers");
        }
        public static string GetTopicLogRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "logs");
        }
        public static string GetMessageRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTopicDirectory(tenantName, productName, componentName, topicName), "messages");
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
        public static string GetTempMessageAckedTopicRootDirectory(string tenantName, string productName, string componentName, string topicName)
        {
            return Path.Combine(GetTempTopicRootDirectory(tenantName, productName, componentName, topicName), "acked");
        }
        #endregion
    }
}
