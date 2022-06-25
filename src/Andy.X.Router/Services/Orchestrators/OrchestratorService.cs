using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using Buildersoft.Andy.X.Model.App.Topics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.Core.Services.Data;

namespace Buildersoft.Andy.X.Router.Services.Orchestrators
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly ILogger<OrchestratorService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IProducerHubService _producerHubService;
        private readonly StorageConfiguration _storageConfiguration;

        private readonly ConcurrentDictionary<string, ITopicDataService> _topicLogServices;

        public OrchestratorService(ILoggerFactory logger, IProducerHubService producerHubService, StorageConfiguration storageConfiguration)
        {
            _logger = logger.CreateLogger<OrchestratorService>();

            _loggerFactory = logger;
            _producerHubService = producerHubService;
            _storageConfiguration = storageConfiguration;

            _topicLogServices = new ConcurrentDictionary<string, ITopicDataService>();
        }

        public ITopicDataService GetTopicDataService(string topicKey)
        {
            if (_topicLogServices.ContainsKey(topicKey) != true)
                return null;

            return _topicLogServices[topicKey];
        }

        public bool InitializeTopicDataService(string tenant, string product, string component, Topic topic)
        {
            string topicKey = ConnectorHelper.GetTopicKey(tenant, product, component, topic.Name);
            if (_topicLogServices.ContainsKey(topicKey))
                return false;

            return _topicLogServices.TryAdd(topicKey, new TopicRocksDbDataService(tenant, product, component, topic.Name, _storageConfiguration));
        }
    }
}
