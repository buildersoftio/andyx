using Buildersoft.Andy.X.IO.Locations;
using System;
using System.IO;

namespace Buildersoft.Andy.X.IO.Services
{
    public static class TenantIOService
    {
        public static bool TryCreateTenantDirectory(string tenant)
        {
            try
            {
                if (Directory.Exists(TenantLocations.GetTenantDirectory(tenant)) == true)
                {
                    // TODO Add logging later
                    return true;
                }

                Directory.CreateDirectory(TenantLocations.GetTenantDirectory(tenant));
                Directory.CreateDirectory(TenantLocations.GetProductRootDirectory(tenant));
                Directory.CreateDirectory(TenantLocations.GetTenantLogsRootDirectory(tenant));
                Directory.CreateDirectory(TenantLocations.GetTenantTokensDirectory(tenant));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCreateProductDirectory(string tenant, string product)
        {
            try
            {
                if (Directory.Exists(TenantLocations.GetProductDirectory(tenant, product)) == true)
                {
                    // TODO Add logging later

                    return true;
                }

                Directory.CreateDirectory(TenantLocations.GetProductDirectory(tenant, product));
                Directory.CreateDirectory(TenantLocations.GetComponentRootDirectory(tenant, product));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCreateComponentDirectory(string tenant, string product, string component)
        {
            try
            {
                if (Directory.Exists(TenantLocations.GetComponentDirectory(tenant, product, component)) == true)
                {
                    // TODO Add logging later

                    return true;
                }

                Directory.CreateDirectory(TenantLocations.GetComponentDirectory(tenant, product, component));
                Directory.CreateDirectory(TenantLocations.GetTopicRootDirectory(tenant, product, component));
                Directory.CreateDirectory(TenantLocations.GetComponentTokenDirectory(tenant, product, component));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCreateTopicDirectory(string tenant, string product, string component, string topic)
        {
            try
            {
                if (Directory.Exists(TenantLocations.GetTopicDirectory(tenant, product, component, topic)) == true)
                {
                    // TODO Add logging later

                    return true;
                }

                Directory.CreateDirectory(TenantLocations.GetProducerRootDirectory(tenant, product, component, topic));
                Directory.CreateDirectory(TenantLocations.GetSubscriptionRootDirectory(tenant, product, component, topic));
                Directory.CreateDirectory(TenantLocations.GetTopicLogRootDirectory(tenant, product, component, topic));
                Directory.CreateDirectory(TenantLocations.GetMessageRootDirectory(tenant, product, component, topic));
                Directory.CreateDirectory(TenantLocations.GetTempTopicRootDirectory(tenant, product, component, topic));
                Directory.CreateDirectory(TenantLocations.GetTempMessageToStoreTopicRootDirectory(tenant, product, component, topic));
                Directory.CreateDirectory(TenantLocations.GetTempMessageUnAckedTopicRootDirectory(tenant, product, component, topic));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCreateSubscriptionDirectory(string tenant, string product, string component, string topic, string subscription)
        {
            try
            {
                if (Directory.Exists(TenantLocations.GetSubscriptionDirectory(tenant, product, component, topic, subscription)) == true)
                {
                    // TODO: Add logging later
                    return true;
                }

                Directory.CreateDirectory(TenantLocations.GetSubscriptionDirectory(tenant, product, component, topic, subscription));
                Directory.CreateDirectory(TenantLocations.GetConsumerRootDirectory(tenant, product, component, topic, subscription));
                Directory.CreateDirectory(TenantLocations.GetSubscriptionLogsDirectory(tenant, product, component, topic, subscription));
                Directory.CreateDirectory(TenantLocations.GetSubscriptionUnackedDirectory(tenant, product, component, topic, subscription));

                //TODO: CREATE db_log FILES HERE.

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCreateConsumerDirectory(string tenant, string product, string component, string topic, string subscription, string consumer)
        {
            try
            {
                if (Directory.Exists(TenantLocations.GetConsumerDirectory(tenant, product, component, topic, subscription, consumer)) == true)
                {
                    // TODO: Add logging later
                    return true;
                }

                Directory.CreateDirectory(TenantLocations.GetConsumerDirectory(tenant, product, component, topic, subscription, consumer));
                Directory.CreateDirectory(TenantLocations.GetConsumerLogsRootDirectory(tenant, product, component, topic, subscription, consumer));

                //TODO: CREATE db_log FILES HERE.

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCreateProducerDirectory(string tenant, string product, string component, string topic, string producerName)
        {
            try
            {
                if (Directory.Exists(ProducerLocations.GetProducerDirectory(tenant, product, component, topic, producerName)) == true)
                {
                    // TODO Add logging later

                    return true;
                }

                Directory.CreateDirectory(ProducerLocations.GetProducerDirectory(tenant, product, component, topic, producerName));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
