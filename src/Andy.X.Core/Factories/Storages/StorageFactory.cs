using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Model.Storages;
using Buildersoft.Andy.X.Model.Storages.Agents;
using System;

namespace Buildersoft.Andy.X.Core.Factories.Storages
{
    public class StorageFactory : IStorageFactory
    {
        public Storage CreateStorage()
        {
            return new Storage();
        }

        public Storage CreateStorage(string storageName, StorageStatus storageStatus, bool isLoadBalanced, int agnetMaxNumber, int agnetMinNumber)
        {
            return new Storage()
            {
                StorageName = storageName,
                AgentMaxNumber = agnetMaxNumber,
                AgentMinNumber = agnetMinNumber,
                StorageStatus = storageStatus,
                IsLoadBalanced = isLoadBalanced,
                Agents = new System.Collections.Concurrent.ConcurrentDictionary<string, Agent>()
            };
        }
    }
}
