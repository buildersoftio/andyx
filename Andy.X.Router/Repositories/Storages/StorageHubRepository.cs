using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Model.Storages;
using Buildersoft.Andy.X.Model.Storages.Agents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Router.Repositories.Storages
{
    public class StorageHubRepository : IStorageHubRepository
    {
        private readonly ILogger<StorageHubRepository> _logger;
        private ConcurrentDictionary<string, Storage> _storages;

        public StorageHubRepository(ILogger<StorageHubRepository> logger)
        {
            _logger = logger;
            _storages = new ConcurrentDictionary<string, Storage>();
        }

        public bool AddAgent(string storageName, string connectionId, Agent agent)
        {
            if (_storages.ContainsKey(storageName) != true)
            {
                _logger.LogError($"Storage with name {storageName} doesn't exists");
                return false;
            }

            return _storages[storageName].Agents.TryAdd(connectionId, agent);

        }

        public bool AddStorage(string storageName, Storage storage)
        {
            return _storages.TryAdd(storageName, storage);
        }

        public Agent GetAgentById(string storageName, string connectionId)
        {
            if (_storages.ContainsKey(storageName) != true)
            {
                _logger.LogError($"Storage with name {storageName} doesn't exists");
                return null;
            }

            if (_storages[storageName].Agents.ContainsKey(connectionId) != true)
            {
                _logger.LogError($"Agent with id '{connectionId}' in storage with name {storageName} doesn't exists");
                return null;
            }

            return _storages[storageName].Agents[connectionId];
        }

        public Agent GetAgentById(string connectionId)
        {
            foreach (var storage in _storages)
            {
                if (storage.Value.Agents.ContainsKey(connectionId))
                    return storage.Value.Agents[connectionId];
            }

            return null;
        }

        public ConcurrentDictionary<string, Agent> GetAgentsByStorageName(string storageName)
        {
            if (_storages.ContainsKey(storageName) != true)
            {
                _logger.LogError($"Storage with name {storageName} doesn't exists");
                return null;
            }

            return _storages[storageName].Agents;
        }

        public Storage GetStorageByName(string storageName)
        {
            if (_storages.ContainsKey(storageName) != true)
            {
                _logger.LogError($"Storage with name {storageName} doesn't exists");
                return null;
            }

            return _storages[storageName];
        }

        public bool RemoveAgent(string storageName, string connectionId)
        {
            if (_storages.ContainsKey(storageName) != true)
            {
                _logger.LogError($"Storage with name {storageName} doesn't exists");
                return false;
            }

            if (_storages[storageName].Agents.ContainsKey(connectionId) != true)
            {
                _logger.LogError($"Agent with id '{connectionId}' in storage with name {storageName} doesn't exists");
                return false;
            }

            return _storages[storageName].Agents.TryRemove(connectionId, out _);
        }

        public bool RemoveStorage(string storageName)
        {
            if (_storages.ContainsKey(storageName) != true)
            {
                _logger.LogError($"Storage with name {storageName} doesn't exists");
                return false;
            }

            return _storages.TryRemove(storageName, out _);
        }
    }
}
