using Buildersoft.Andy.X.Model.Storages.Agents;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.Storages
{
    public class Storage
    {
        public Guid StorageId { get; private set; }
        public string StorageName { get; set; }
        public StorageStatus StorageStatus { get; set; }
        public ConcurrentDictionary<string, Agent> Agents { get; set; }
        public int ActiveAgentIndex { get; set; }


        public bool IsLoadBalanced { get; set; }

        // If IsLoadBalanced is false AgentMaxNumber will be default number of data storage agents connected to node.
        public int AgnetMaxNumber { get; set; }
        public int AgnetMinNumber { get; set; }


        public Storage()
        {
            StorageId = Guid.NewGuid();
            Agents = new ConcurrentDictionary<string, Agent>();
            ActiveAgentIndex = 0;
        }
    }
}
