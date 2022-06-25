using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.Model.App.Topics;

namespace Buildersoft.Andy.X.Core.Abstractions.Orchestrators
{
    public interface IOrchestratorService
    {
        ITopicDataService GetTopicDataService(string topicKey);
        bool InitializeTopicDataService(string tenant, string product, string component, Topic topic);
    }
}
