using Buildersoft.Andy.X.Model.Storages;
using Buildersoft.Andy.X.Model.Storages.Agents;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages
{
    public interface IStorageHubRepository
    {
        bool AddStorage(string storageName, Storage storage);
        bool AddAgent(string storageName, string connectionId, Agent agent);
        bool RemoveStorage(string storageName);
        bool RemoveAgent(string storageName, string connectionId);
        bool RemoveAgent(string connectionId);

        Storage GetStorageByName(string storageName);
        Agent GetAgentById(string storageName, string connectionId);
        Agent GetAgentById(string connectionId);

        ConcurrentDictionary<string, Agent> GetAgentsByStorageName(string storageName);
    }
}
