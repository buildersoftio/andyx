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
        private readonly IProducerHubService _producerHubService;

        private readonly ConcurrentDictionary<string, TopicSynchronizerProcess> _topicProcesses;

        public OrchestratorService(ILogger<OrchestratorService> logger, IProducerHubService producerHubService)
        {
            _logger = logger;
            _producerHubService = producerHubService;

            _topicProcesses = new ConcurrentDictionary<string, TopicSynchronizerProcess>();
        }

        public void AddTopicStorageSynchronizer(string tenant, string product, string component, Topic topic)
        {
            string processKey = ConnectorHelper.GetTopicSynchronizerKey(tenant, product, component, topic.Name);
            if (_topicProcesses.ContainsKey(processKey))
                return;

            _topicProcesses.TryAdd(processKey, new TopicSynchronizerProcess(_logger) { Tenant = tenant, Product = product, Component = component, Topic = topic });
            _topicProcesses[processKey].StartProcess();
        }

        public void StartTopicStorageSynchronizerProcess(string topicKey)
        {
            _topicProcesses[topicKey].StartProcess();
        }
    }
}
