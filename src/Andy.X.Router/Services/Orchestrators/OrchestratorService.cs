using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Core.Synchronizers;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Router.Services.Orchestrators
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly ILogger<OrchestratorService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IProducerHubService _producerHubService;

        private readonly ConcurrentDictionary<string, TopicSynchronizerProcess> _topicProcesses;
        private readonly ConcurrentDictionary<string, SubscriptionSynchronizerProcess> _subscriptionProcesses;

        public OrchestratorService(ILoggerFactory logger, IProducerHubService producerHubService)
        {
            _logger = logger.CreateLogger<OrchestratorService>();

            _loggerFactory = logger;
            _producerHubService = producerHubService;

            _topicProcesses = new ConcurrentDictionary<string, TopicSynchronizerProcess>();
            _subscriptionProcesses = new ConcurrentDictionary<string, SubscriptionSynchronizerProcess>();
        }

        public void AddTopicStorageSynchronizer(string tenant, string product, string component, Topic topic)
        {
            string processKey = ConnectorHelper.GetTopicSynchronizerKey(tenant, product, component, topic.Name);
            if (_topicProcesses.ContainsKey(processKey))
                return;

            _topicProcesses.TryAdd(processKey,
                new TopicSynchronizerProcess(_loggerFactory.CreateLogger<TopicSynchronizerProcess>()) { Tenant = tenant, Product = product, Component = component, Topic = topic });
            _subscriptionProcesses.TryAdd(processKey,
                new SubscriptionSynchronizerProcess(_loggerFactory.CreateLogger<SubscriptionSynchronizerProcess>()) { Tenant = tenant, Product = product, Component = component, Topic = topic });

            _topicProcesses[processKey].StartProcess();
        }

        public void StartSubscriptionSynchronizerProcess(string topicKey)
        {
            _subscriptionProcesses[topicKey].StartProcess();
        }

        public void StartTopicStorageSynchronizerProcess(string topicKey)
        {
            _topicProcesses[topicKey].StartProcess();
        }
    }
}
