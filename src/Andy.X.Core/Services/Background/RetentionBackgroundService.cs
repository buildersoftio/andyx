using Buildersoft.Andy.X.Core.Abstractions.Repositories;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.CoreState;
using Buildersoft.Andy.X.Core.Abstractions.Services.Background;
using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.Core.Contexts.Subscriptions;
using Buildersoft.Andy.X.Core.Services.Outbound.Connectors;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Core.Components;
using Buildersoft.Andy.X.Model.Entities.Core.Products;
using Buildersoft.Andy.X.Model.Entities.Core.Tenants;
using Buildersoft.Andy.X.Model.Entities.Core.Topics;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Buildersoft.Andy.X.Core.Services.Background
{
    public class RetentionBackgroundService : IRetentionService
    {
        private readonly ILogger<RetentionBackgroundService> _logger;
        private readonly string _tenant;
        private readonly string _product;
        private readonly string _component;
        private readonly string _topic;

        private readonly StorageConfiguration _storageConfiguration;

        private readonly ICoreRepository _coreRepository;
        private readonly ITenantStateRepository _tenantStateRepository;
        private readonly ITopicDataService<Message> _topicDataService;

        private readonly Timer _backgroundTimer;

        private bool isServiceRunning = false;

        public RetentionBackgroundService(
            ILogger<RetentionBackgroundService> logger,
            string tenant,
            string product,
            string component,
            string topic,
            ICoreRepository coreRepository,
            StorageConfiguration storageConfiguration,
            ITenantStateRepository tenantStateRepository,
            ITopicDataService<Message> topicDataService)
        {
            _logger = logger;
            _tenant = tenant;
            _product = product;
            _component = component;
            _topic = topic;
            _coreRepository = coreRepository;
            _storageConfiguration = storageConfiguration;
            _tenantStateRepository = tenantStateRepository;
            _topicDataService = topicDataService;

            // initializing background retention timer
            _backgroundTimer = new Timer()
            {
                Interval = new TimeSpan(0, storageConfiguration.RetentionBackgroundServiceIntervalInMinutes, 0).TotalMilliseconds,
                AutoReset = true,
            };

            _backgroundTimer.Elapsed += BackgroundTimer_Elapsed;
            StartService();

            logger.LogInformation($"Background service for retention at {tenant}/{product}/{component}/{topic} has been initiaized");
        }

        private void BackgroundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _backgroundTimer?.Stop();

            //_logger.LogInformation($"Background service for {_tenant}/{_product}/{_component}/{_topic} is triggered");

            var tenant = _coreRepository.GetTenant(_tenant);
            var product = _coreRepository.GetProduct(tenant.Id, _product);
            var component = _coreRepository.GetComponent(tenant.Id, product.Id, _component);
            var topic = _coreRepository.GetTopic(component.Id, _topic);

            var componentRetentions = _coreRepository.GetComponentRetentions(component.Id);

            if (componentRetentions.Count != 0)
            {
                // add component retention logic
                AnalyzeAndDeleteMessagesBasedOnComponent(componentRetentions, topic);

                _backgroundTimer.Start();
                return;
            }

            var productRetentions = _coreRepository.GetProductRetentions(product.Id);
            if (productRetentions.Count != 0)
            {
                // add product retention logic
                AnalyzeAndDeleteMessagesBasedOnProduct(productRetentions, topic);
                _backgroundTimer.Start();
                return;
            }

            var tenantRetentions = _coreRepository.GetTenantRetentions(tenant.Id);
            if (tenantRetentions.Count != 0)
            {
                // add tenant retention logic
                AnalyzeAndDeleteMessagesBasedOnTenant(tenantRetentions, topic);
                _backgroundTimer.Start();
                return;
            }

            _backgroundTimer.Start();
        }

        private void AnalyzeAndDeleteMessagesBasedOnComponent(List<ComponentRetention> componentRetentions, Topic topicFromConfig)
        {
            componentRetentions = componentRetentions.OrderBy(a => a.Type).ToList();
            var topic = _tenantStateRepository.GetTopic(_tenant, _product, _component, _topic);

            var subscriptions = _coreRepository.GetSubscriptions(topicFromConfig.Id).Select(x => x.Name);

            foreach (var retention in componentRetentions)
            {
                if (retention.Type == Model.Entities.Core.RetentionType.HARD_TTL)
                {
                    // start reading from the oldest message in this topic.
                    for (int i = 0; i < _storageConfiguration.RetentionBulkMessagesCountToAnalyze; i++)
                    {
                        var nextIdToDelete = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        var message = _topicDataService.Get(nextIdToDelete);
                        if (message is null)
                            break;

                        // check sent date with ttl
                        if (message.StoredDate.AddMinutes(retention.TimeToLiveInMinutes) < DateTimeOffset.UtcNow)
                        {
                            // delete this message
                            _topicDataService.Delete(nextIdToDelete);

                            // update the state
                            topic.TopicStates.MarkDeleteEntryPosition = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (retention.Type == Model.Entities.Core.RetentionType.SOFT_TTL)
                {
                    // get the list of subscriptions and check the current consumed entry id with oldest entry if of the topic.
                    List<long> subscriptionEntries = GetSubscriptionEntryConsumptionId(subscriptions);
                    for (int i = 0; i < _storageConfiguration.RetentionBulkMessagesCountToAnalyze; i++)
                    {
                        var nextIdToDelete = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        // checking if all subscriptions have read this message

                        var messageIsConsumed = IsMessageConsumedFromAllSubscriptions(nextIdToDelete, subscriptionEntries);
                        if (messageIsConsumed != true)
                            break;

                        var message = _topicDataService.Get(nextIdToDelete);
                        if (message is null)
                            break;

                        // check sent date with ttl
                        if (message.StoredDate.AddMinutes(retention.TimeToLiveInMinutes) < DateTimeOffset.UtcNow)
                        {
                            // delete this message
                            _topicDataService.Delete(nextIdToDelete);

                            // update the state
                            topic.TopicStates.MarkDeleteEntryPosition = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private void AnalyzeAndDeleteMessagesBasedOnProduct(List<ProductRetention> productRetentions, Topic topicFromConfig)
        {
            productRetentions = productRetentions.OrderBy(a => a.Type).ToList();
            var topic = _tenantStateRepository.GetTopic(_tenant, _product, _component, _topic);

            var subscriptions = _coreRepository.GetSubscriptions(topicFromConfig.Id).Select(x => x.Name);

            foreach (var retention in productRetentions)
            {
                if (retention.Type == Model.Entities.Core.RetentionType.HARD_TTL)
                {
                    // start reading from the oldest message in this topic.
                    for (int i = 0; i < _storageConfiguration.RetentionBulkMessagesCountToAnalyze; i++)
                    {
                        var nextIdToDelete = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        var message = _topicDataService.Get(nextIdToDelete);
                        if (message is null)
                            break;

                        // check sent date with ttl
                        if (message.StoredDate.AddMinutes(retention.TimeToLiveInMinutes) < DateTimeOffset.UtcNow)
                        {
                            // delete this message
                            _topicDataService.Delete(nextIdToDelete);

                            // update the state
                            topic.TopicStates.MarkDeleteEntryPosition = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (retention.Type == Model.Entities.Core.RetentionType.SOFT_TTL)
                {
                    // get the list of subscriptions and check the current consumed entry id with oldest entry if of the topic.
                    List<long> subscriptionEntries = GetSubscriptionEntryConsumptionId(subscriptions);
                    for (int i = 0; i < _storageConfiguration.RetentionBulkMessagesCountToAnalyze; i++)
                    {
                        var nextIdToDelete = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        // checking if all subscriptions have read this message

                        var messageIsConsumed = IsMessageConsumedFromAllSubscriptions(nextIdToDelete, subscriptionEntries);
                        if (messageIsConsumed != true)
                            break;

                        var message = _topicDataService.Get(nextIdToDelete);
                        if (message is null)
                            break;

                        // check sent date with ttl
                        if (message.StoredDate.AddMinutes(retention.TimeToLiveInMinutes) < DateTimeOffset.UtcNow)
                        {
                            // delete this message
                            _topicDataService.Delete(nextIdToDelete);

                            // update the state
                            topic.TopicStates.MarkDeleteEntryPosition = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void AnalyzeAndDeleteMessagesBasedOnTenant(List<TenantRetention> tenantRetentions, Topic topicFromConfig)
        {
            tenantRetentions = tenantRetentions.OrderBy(a => a.Type).ToList();
            var topic = _tenantStateRepository.GetTopic(_tenant, _product, _component, _topic);
            
            var subscriptions = _coreRepository.GetSubscriptions(topicFromConfig.Id).Select(x => x.Name);

            foreach (var retention in tenantRetentions)
            {
                if (retention.Type == Model.Entities.Core.RetentionType.HARD_TTL)
                {
                    // start reading from the oldest message in this topic.
                    for (int i = 0; i < _storageConfiguration.RetentionBulkMessagesCountToAnalyze; i++)
                    {
                        var nextIdToDelete = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        var message = _topicDataService.Get(nextIdToDelete);
                        if (message is null)
                            break;

                        // check sent date with ttl
                        if (message.StoredDate.AddMinutes(retention.TimeToLiveInMinutes) < DateTimeOffset.UtcNow)
                        {
                            // delete this message
                            _topicDataService.Delete(nextIdToDelete);

                            // update the state
                            topic.TopicStates.MarkDeleteEntryPosition = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (retention.Type == Model.Entities.Core.RetentionType.SOFT_TTL)
                {
                    // get the list of subscriptions and check the current consumed entry id with oldest entry if of the topic.
                    List<long> subscriptionEntries = GetSubscriptionEntryConsumptionId(subscriptions);
                    for (int i = 0; i < _storageConfiguration.RetentionBulkMessagesCountToAnalyze; i++)
                    {
                        var nextIdToDelete = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        // checking if all subscriptions have read this message

                        var messageIsConsumed = IsMessageConsumedFromAllSubscriptions(nextIdToDelete, subscriptionEntries);
                        if (messageIsConsumed != true)
                            break;

                        var message = _topicDataService.Get(nextIdToDelete);
                        if (message is null)
                            break;

                        // check sent date with ttl
                        if (message.StoredDate.AddMinutes(retention.TimeToLiveInMinutes) < DateTimeOffset.UtcNow)
                        {
                            // delete this message
                            _topicDataService.Delete(nextIdToDelete);

                            // update the state
                            topic.TopicStates.MarkDeleteEntryPosition = topic.TopicStates.MarkDeleteEntryPosition + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private bool IsMessageConsumedFromAllSubscriptions(long nextIdToDelete, List<long> subscriptionEntries)
        {
            return !subscriptionEntries
                .Any(x => nextIdToDelete >= x);
        }

        private List<long> GetSubscriptionEntryConsumptionId(IEnumerable<string> subscriptions)
        {
            var results = new List<long>();
            foreach (var subscription in subscriptions)
            {
                using (var subDbContext = new SubscriptionPositionContext(_tenant, _product, _component, _topic, subscription))
                {
                    var position = subDbContext.CurrentPosition.OrderBy(x => x.SubscriptionName).FirstOrDefault();
                    results.Add(position.ReadEntryPosition);
                }
            }
            return results;
        }


        public void StartService()
        {
            isServiceRunning = true;
            _backgroundTimer.Start();
        }

        public void StopService()
        {
            isServiceRunning = false;
            _backgroundTimer.Stop();
        }

        public bool IsServiceRunning()
        {
            return isServiceRunning;
        }
    }
}
