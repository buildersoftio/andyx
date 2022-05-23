using Buildersoft.Andy.X.Model.App.Topics;

namespace Buildersoft.Andy.X.Core.Abstractions.Orchestrators
{
    public interface IOrchestratorService
    {
        public void AddTopicStorageSynchronizer(string tenant, string product, string component, Topic topic);

        public void StartTopicStorageSynchronizerProcess(string topicKey);
    }
}
