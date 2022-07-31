using Buildersoft.Andy.X.Core.Abstractions.Service.Subscriptions;
using Buildersoft.Andy.X.Core.Abstractions.Service.Producers;
using Buildersoft.Andy.X.Core.Abstractions.Services;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api.Lineage;
using Buildersoft.Andy.X.Model.App.Lineage;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Services.Api.Lineage
{
    public class StreamLineageService : IStreamLineageService
    {
        private readonly ILogger<StreamLineageService> _logger;
        private readonly ITenantService _tenantRepository;
        private readonly ISubscriptionHubRepository _consumerHubRepository;
        private readonly IProducerHubRepository _producerHubRepository;

        public StreamLineageService(
            ILogger<StreamLineageService> logger,
            ITenantService tenantRepository,
            ISubscriptionHubRepository consumerHubRepository,
            IProducerHubRepository producerHubRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _consumerHubRepository = consumerHubRepository;
            _producerHubRepository = producerHubRepository;
        }

        public List<StreamLineage> GetStreamLineages(string tenant)
        {
            var streams = new List<StreamLineage>();

            if (_tenantRepository.GetTenant(tenant) == null)
                return streams;

            var products = _tenantRepository.GetTenant(tenant).Products.Keys;

            foreach (var product in products)
            {
                streams.AddRange(GetStreamLineages(tenant, product));
            }
            return streams;
        }

        public List<StreamLineage> GetStreamLineages(string tenant, string product)
        {
            var streams = new List<StreamLineage>();

            if (_tenantRepository.GetProduct(tenant, product) == null)
                return streams;

            var components = _tenantRepository.GetProduct(tenant, product).Components.Keys;

            foreach (var component in components)
            {
                streams.AddRange(GetStreamLineages(tenant, product, component));
            }

            return streams;
        }

        public List<StreamLineage> GetStreamLineages(string tenant, string product, string component)
        {
            var streams = new List<StreamLineage>();

            if (_tenantRepository.GetComponent(tenant, product, component) == null)
                return streams;

            var topics = _tenantRepository.GetComponent(tenant, product, component).Topics.Keys;

            foreach (var topic in topics)
            {
                streams.Add(GetStreamLineage(tenant, product, component, topic));
            }

            return streams;
        }

        public StreamLineage GetStreamLineage(string tenant, string product, string component, string topic)
        {
            return new StreamLineage()
            {
                Producers = _producerHubRepository.GetProducers(tenant, product, component, topic).Values.ToList(),
                Topic = topic,
                TopicPhysicalPath = $"{tenant}/{product}/{component}/{topic}",
                Subscriptions = _consumerHubRepository.GetSubscriptionsByTopic(tenant, product, component, topic).Values.ToList()
            };
        }
    }
}
