using Buildersoft.Andy.X.Core.Abstractions.Factories.Tenants;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Model.App.Topics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class TopicService : ITopicService
    {
        private readonly ILogger<TopicService> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantFactory _tenantFactory;

        public TopicService(ILogger<TopicService> logger, ITenantRepository tenantRepository, ITenantFactory tenantFactory)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _tenantFactory = tenantFactory;
        }

        public bool AddTopic(string tenantName, string productName, string componentName, string topicName)
        {
            return _tenantRepository
                .AddTopic(tenantName, productName, componentName, topicName, _tenantFactory.CreateTopic(topicName));
        }

        public Topic GetTopic(string tenantName, string productName, string componentName, string topicName)
        {
            try
            {
                return _tenantRepository
                    .GetTopic(tenantName, productName, componentName, topicName);
            }
            catch (Exception)
            {
                // TODO Log later
                return null;
            }
        }

        public List<Topic> GetTopics(string tenantName, string productName, string componentName)
        {
            var result = new List<Topic>();
            try
            {
                var topics = _tenantRepository.GetTopics(tenantName, productName, componentName);
                foreach (var topic in topics)
                {
                    result.Add(new Topic() { Id = topic.Value.Id, Name = topic.Value.Name, TopicSettings = topic.Value.TopicSettings });
                }
            }
            catch (Exception)
            {
                // TODO Log later
            }
            return result;
        }
    }
}
