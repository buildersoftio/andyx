using Buildersoft.Andy.X.Model.Entities.Core;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Producers;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Buildersoft.Andy.X.Model.Subscriptions;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.CoreState
{
    public interface ICoreService
    {
        bool CreateTenant(string tenantName);
        bool DeleteTenant(string tenantName, bool notifyCluster = true);
        bool ActivateTenant(string tenantName);
        bool DeactivateTenant(string tenantName);
        bool CreateTenant(string tenantName, bool isProductCreation, bool isEncryptionEnabled, bool isAuthorizationEnabled);
        bool UpdateTenantSettings(string tenantName, bool isProductCreation, bool isEncryptionEnabled, bool isAuthorizationEnabled, bool notifyCluster = true);

        bool CreateTenantToken(string tenant, string description, DateTimeOffset expireDate, List<TenantTokenRole> tenantTokenRoles, out Guid id, out string secret, bool notifyCluster = true);
        bool RevokeTenantToken(string tenant, Guid key, bool notifyCluster = true);
        bool DeleteTenantToken(string tenant, Guid key, bool notifyCluster = true);

        bool CreateTenantRetention(string tenant, string name, RetentionType retentionType, long timeToLive, bool notifyCluster = true);
        bool UpdateTenantRetention(string tenant, long retentionId, string name, long timeToLive, bool notifyCluster = true);
        bool DeleteTenantRetention(string tenant, long retentionId, bool notifyCluster = true);

        bool CreateProduct(string tenant, string productName, string description);
        bool CreateProduct(string tenant, string productName, string description, bool isAuthorizationEnabled);
        bool DeleteProduct(string tenant, string product, bool notifyCluster = true);
        bool UpdateProduct(string tenant, string product, string description);
        bool UpdateProductSettings(string tenant, string product, bool isAuthorizationEnabled, bool notifyCluster = true);

        bool CreateProductToken(string tenant, string product, string description, DateTimeOffset expireDate, List<ProductTokenRole> productTokenRoles, out Guid id, out string secret, bool notifyCluster = true);
        bool RevokeProductToken(string tenant, string product, Guid id, bool notifyCluster = true);
        bool DeleteProductToken(string tenant, string product, Guid id, bool notifyCluster = true);

        bool CreateProductRetention(string tenant, string product, string name, RetentionType retentionType, long timeToLive, bool notifyCluster = true);
        bool UpdateProductRetention(string tenant, string product, long retentionId, string name, long timeToLive, bool notifyCluster = true);
        bool DeleteProductRetention(string tenant, string product, long retentionId, bool notifyCluster = true);

        bool CreateComponent(string tenant, string product, string componentName, string description);
        bool CreateComponent(string tenant, string product, string componentName, string description, bool isTopicAutomaticCreation, bool isSchemaValidationEnabled, bool isAuthorizationEnabled, bool isSubscriptionAllowToCreate, bool isProducerAllowToCreate);
        bool DeleteComponent(string tenant, string product, string component, bool notifyCluster = true);
        bool UpdateComponent(string tenant, string product, string component, string description);
        bool UpdateComponentSettings(string tenant, string product, string componentName, bool isTopicAutomaticCreation, bool isSchemaValidationEnabled, bool isAuthorizationEnabled, bool isSubscriptionAllowToCreate, bool notifyCluster = true);

        bool CreateComponentToken(string tenant, string product, string component, string description, string issuedFor, DateTimeOffset expireDate, List<ComponentTokenRole> componentTokenRoles, out Guid id, out string secret, bool notifyCluster = true);
        bool RevokeComponentToken(string tenant, string product, string component, Guid id, bool notifyCluster = true);
        bool DeleteComponentToken(string tenant, string product, string component, Guid id, bool notifyCluster = true);

        bool CreateComponentRetention(string tenant, string product, string component, string name, RetentionType retentionType, long timeToLive, bool notifyCluster = true);
        bool UpdateComponentRetention(string tenant, string product, string component, long retentionId, string name, long timeToLive, bool notifyCluster = true);
        bool DeleteComponentRetention(string tenant, string product, string component, long retentionId, bool notifyCluster = true);

        bool CreateTopic(string tenant, string product, string component, string topic, string description);
        bool CreateTopic(string tenant, string product, string component, string topic, string description, TopicSettings topicSettings);
        bool UpdateTopicSettings(string tenant, string product, string component, string topic, TopicSettings topicSettings, bool notifyCluster = true);
        bool UpdateTopic(string tenant, string product, string component, string topic, string description);
        bool DeleteTopic(string tenant, string product, string component, string topic, bool notifyCluster = true);

        bool CreateSubscription(string tenant, string product, string component, string topic, string subscription, SubscriptionType type, SubscriptionMode mode, InitialPosition initialPosition);
        bool DeleteSubscription(string tenant, string product, string component, string topic, string subscription);

        bool CreateProducer(string tenant, string product, string component, string topic, string producer, string description, ProducerInstanceType producerInstanceType, bool notifyCluster = true);
        bool DeleteProducer(string tenant, string product, string component, string topic, string producer, bool notifyCluster = true);
    }
}
