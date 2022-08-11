﻿using Buildersoft.Andy.X.Model.App.Components;
using Buildersoft.Andy.X.Model.App.Products;
using Buildersoft.Andy.X.Model.App.Tenants;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Subscriptions;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services
{
    public interface ITenantService
    {
        bool AddTenant(string tenantName, Tenant tenant, bool notifyOtherNodes = true);
        bool AddProduct(string tenant, string productName, Product product, bool notifyOtherNodes = true);
        bool AddComponent(string tenant, string product, string componentName, Component component, bool notifyOtherNodes = true);
        bool AddTopic(string tenant, string product, string component, string topicName, Topic topic, bool notifyOtherNodes = true);

        public bool AddSubscriptionConfiguration(string tenant, string product, string component, string topicName, string subscriptionName, Subscription subscription);


        Tenant GetTenant(string tenant);
        Product GetProduct(string tenant, string product);
        Component GetComponent(string tenant, string product, string component);
        Topic GetTopic(string tenant, string product, string component, string topic);

        TenantToken GetTenantToken(string tenant, string token);
        ComponentToken GetComponentToken(string tenant, string product, string component, string componentToken);
        List<ComponentToken> GetComponentTokens(string tenant, string product, string component);

        ConcurrentDictionary<string, Tenant> GetTenants();
        ConcurrentDictionary<string, Product> GetProducts(string tenant);
        ConcurrentDictionary<string, Component> GetComponents(string tenant, string product);
        ConcurrentDictionary<string, Topic> GetTopics(string tenant, string product, string component);

        bool AddTenantToken(string tenant, TenantToken token);
        bool RemoveTenantToken(string tenant, string token);
        bool AddComponentToken(string tenant, string product, string component, ComponentToken componentToken);
        bool RemoveComponentToken(string tenant, string product, string component, string token);

        bool AddComponentRetention(string tenant, string product, string component, ComponentRetention componentRetention);
        ComponentRetention GetComponentRetention(string tenant, string product, string component);


        void AddTenantFromApi(TenantConfiguration tenantConfig);
    }
}