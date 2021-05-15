using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Model.Storages;
using Buildersoft.Andy.X.Model.Storages.Agents;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Storages
{
    public class StorageHub : Hub<IStorageHub>
    {
        private readonly ILogger<StorageHub> logger;
        private readonly IStorageHubRepository storageHubRepository;
        private readonly IStorageFactory storageFactory;
        private readonly IAgentFactory agentFactory;

        public StorageHub(ILogger<StorageHub> logger, IStorageHubRepository storageHubRepository, IStorageFactory storageFactory, IAgentFactory agentFactory)
        {
            this.logger = logger;
            this.storageHubRepository = storageHubRepository;
            this.storageFactory = storageFactory;
            this.agentFactory = agentFactory;
        }

        public override Task OnConnectedAsync()
        {
            Storage storageToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;
            // Send Basic info of DATA STORAGE from HEADER

            // Header Keys

            // x-andyx-storage-name
            // x-andyx-storage-status => string
            // x-andyx-storage-agent-max => int
            // x-andyx-storage-agent-min => int
            // x-andyx-storage-agent-loadbalanced => bool

            // Check if this storage is already connected.
            string storageName = headers["x-andyx-storage-name"].ToString();
            if (storageHubRepository.GetStorageByName(headers["x-andyx-storage-name"].ToString()) == null)
            {
                StorageStatus storageStatus;
                var stroageParsed = Enum.TryParse<StorageStatus>(headers["x-andyx-storage-status"].ToString(), true, out storageStatus);
                if (stroageParsed != true)
                {
                    storageStatus = StorageStatus.Blocked;
                    return base.OnDisconnectedAsync(new Exception($"Invalid status code '{headers["x-andyx-storage-status"]}'"));
                }
                int agentMaxValue = Convert.ToInt32(headers["x-andyx-storage-agent-max"].ToString());
                int agentMinValue = Convert.ToInt32(headers["x-andyx-storage-agent-min"].ToString());
                bool isLoadBalanced = Convert.ToBoolean(headers["x-andyx-storage-agent-loadbalanced"].ToString());

                storageToRegister = storageFactory.CreateStorage(storageName, storageStatus, isLoadBalanced, agentMaxValue, agentMinValue);
                storageHubRepository.AddStorage(storageName, storageToRegister);
            }

            storageToRegister = storageHubRepository.GetStorageByName(headers["x-andyx-storage-name"].ToString());
            if (storageToRegister.Agents.Count - 1 == storageToRegister.AgnetMaxNumber)
                return base.OnDisconnectedAsync(new Exception($"There are '{storageToRegister.AgnetMaxNumber}' agents connected, connection refused"));

            Agent agentToRegister = agentFactory.CreateAgent(clientConnectionId, $"{storageName}-{Guid.NewGuid()}");
            storageHubRepository.AddAgent(storageName, clientConnectionId, agentToRegister);

            Clients.Caller.StorageConnected(new Model.Storages.Events.Agents.AgentConnectedDetails()
            {
                AgentId = agentToRegister.AgentId,
                ConnectionId = Guid.NewGuid(),
                Agent = agentToRegister.AgentName
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            storageHubRepository.RemoveAgent(clientConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
