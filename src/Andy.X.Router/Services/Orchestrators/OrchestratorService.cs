using Buildersoft.Andy.X.Core.Abstractions.Orchestrators;
using Buildersoft.Andy.X.Core.Abstractions.Services.Producers;
using Microsoft.Extensions.Logging;

namespace Buildersoft.Andy.X.Router.Services.Orchestrators
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly ILogger<OrchestratorService> _logger;
        private readonly IProducerHubService _producerHubService;

        public OrchestratorService(ILogger<OrchestratorService> logger, IProducerHubService producerHubService)
        {
            _logger = logger;
            _producerHubService = producerHubService;
        }
    }
}
