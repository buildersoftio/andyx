using Buildersoft.Andy.X.Model.Entities.Core;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
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
        bool DeleteTenant(string tenantName);
        bool ActivateTenant(string tenantName);
        bool DeactivateTenant(string tenantName);
        bool CreateTenant(string tenantName, bool isProductCreation, bool isEncryptionEnabled, bool isAuthorizationEnabled);
        bool UpdateTenantSettings(string tenantName, bool isProductCreation, bool isEncryptionEnabled, bool isAuthorizationEnabled);

        bool CreateTenantToken(string tenant, string description, DateTimeOffset expireDate, List<TenantTokenRole> tenantTokenRoles,out Guid id, out string secret);
        bool RevokeTenantToken(string tenant, Guid id);
        bool DeleteTenantToken(string tenant, Guid id);

        bool CreateTenantRetention(string tenant, string name, RetentionType retentionType, long timeToLive);
        bool UpdateTenantRetention(string tenant, long retentionId, string name, long timeToLive);
        bool DeleteTenantRetention(string tenant, long retentionId);

        bool CreateProduct(string tenant, string productName, string description);
        bool CreateProduct(string tenant, string productName, string description, bool isAuthorizationEnabled);
        bool DeleteProduct(string tenant, string product);
        bool UpdateProduct(string tenant, string product, string description);
        bool UpdateProductSettings(string tenant, string product, bool isAuthorizationEnabled);

        bool CreateProductToken(string tenant, string product, string description, DateTimeOffset expireDate, List<ProductTokenRole> productTokenRoles, out Guid id, out string secret);
        bool RevokeProductToken(string tenant, string product, Guid id);
        bool DeleteProductToken(string tenant, string product, Guid id);

        bool CreateProductRetention(string tenant, string product, string name, RetentionType retentionType, long timeToLive);
        bool UpdateProductRetention(string tenant, string product, long retentionId, string name, long timeToLive);
        bool DeleteProductRetention(string tenant, string product, long retentionId);

        bool CreateComponent(string tenant, string product, string componentName, string description);
        bool CreateComponent(string tenant, string product, string componentName, string description, bool isTopicAutomaticCreation, bool isSchemaValidationEnabled, bool isAuthorizationEnabled, bool isSubscriptionAllowToCreate);
        bool DeleteComponent(string tenant, string product, string component);
        bool UpdateComponent(string tenant, string product, string component, string description);
        bool UpdateComponentSettings(string tenant, string product, string componentName, bool isTopicAutomaticCreation, bool isSchemaValidationEnabled, bool isAuthorizationEnabled, bool isSubscriptionAllowToCreate);

        bool CreateComponentToken(string tenant, string product, string component, string description, string issuedFor,DateTimeOffset expireDate, List<ComponentTokenRole> componentTokenRoles, out Guid id, out string secret);
        bool RevokeComponentToken(string tenant, string product, string component, Guid id);
        bool DeleteComponentToken(string tenant, string product, string component, Guid id);

        bool CreateComponentRetention(string tenant, string product, string component, string name, RetentionType retentionType, long timeToLive);
        bool UpdateComponentRetention(string tenant, string product, string component, long retentionId, string name, long timeToLive);
        bool DeleteComponentRetention(string tenant, string product, string component, long retentionId);

        bool CreateTopic(string tenant, string product, string component, string topic, string description);
        bool CreateTopic(string tenant, string product, string component, string topic, string description, TopicSettings topicSettings);
        bool UpdateTopicSettings(string tenant, string product, string component, string topic, TopicSettings topicSettings);
        bool UpdateTopic(string tenant, string product, string component, string topic, string description);
        bool DeleteTopic(string tenant, string product, string component, string topic);

        bool CreateSubscription(string tenant, string product, string component, string topic, string subscription, SubscriptionType type, SubscriptionMode mode, InitialPosition initialPosition );
        bool DeleteSubscription(string tenant, string product, string component, string topic, string subscription);
    }
}
