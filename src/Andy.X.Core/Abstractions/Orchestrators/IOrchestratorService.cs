using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.Model.App.Topics;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Entities.Clusters;
using Buildersoft.Andy.X.Model.Entities.Storages;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Abstractions.Orchestrators
{
    public interface IOrchestratorService
    {
        // Write RocksDb Connectors for topics
        ITopicDataService<Message> GetTopicDataService(string topicKey);
        bool InitializeTopicDataService(string tenant, string product, string component, Topic topic);

        // Read RocksDb Connectors for topics
        ITopicReadonlyDataService<Message> GetTopicReadonlyDataService(string topicKey);
        bool InitializeTopicReadonlyDataService(string tenant, string product, string component, Topic topic);


        // Write Subscription Unacknowledged Message to RocksDb
        ITopicDataService<UnacknowledgedMessage> GetSubscriptionUnackedDataService(string subscriptionKey);
        bool InitializeSubscriptionUnackedDataService(string tenant, string product, string component, string topic, string subscription);

        // Read Subscription Unacknowledged Message to RocksDb
        ITopicReadonlyDataService<UnacknowledgedMessage> GetSubscriptionUnackedReadonlyDataService(string subscriptionKey);
        bool InitializeSubscriptionUnackedReadonlyDataService(string tenant, string product, string component, string topic, string subscription);

        // Data services for Cluster
        ITopicDataService<ClusterChangeLog> GetClusterDataService(string nodeId);
        ConcurrentDictionary<string, ITopicDataService<ClusterChangeLog>> GetClusterDataServices();
        void InitializeClusterDataService(Replica replica);
    }
}
