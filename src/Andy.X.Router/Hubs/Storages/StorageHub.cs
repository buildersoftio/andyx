using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Hubs.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Repositories.Storages;
using Buildersoft.Andy.X.Core.Abstractions.Services.Consumers;
using Buildersoft.Andy.X.Core.Abstractions.Services.Storages;
using Buildersoft.Andy.X.Model.App.Messages;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Storages;
using Buildersoft.Andy.X.Model.Storages.Agents;
using Buildersoft.Andy.X.Model.Storages.Requests.Components;
using Buildersoft.Andy.X.Model.Storages.Requests.Tenants;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Storages
{
    public class StorageHub : Hub<IStorageHub>
    {
        private readonly ILogger<StorageHub> logger;
        private readonly CredentialsConfiguration credentialsConfiguration;
        private readonly IStorageHubRepository storageHubRepository;
        private readonly ITenantRepository tenantMemoryRepository;
        private readonly IStorageFactory storageFactory;
        private readonly IAgentFactory agentFactory;
        private readonly IConsumerHubService consumerHubService;

        public StorageHub(ILogger<StorageHub> logger,
            CredentialsConfiguration credentialsConfiguration,
            IStorageHubRepository storageHubRepository,
            ITenantRepository tenantMemoryRepository,
            IStorageFactory storageFactory,
            IAgentFactory agentFactory,
            IConsumerHubService consumerHubService)
        {
            this.logger = logger;
            this.credentialsConfiguration = credentialsConfiguration;
            this.storageHubRepository = storageHubRepository;
            this.tenantMemoryRepository = tenantMemoryRepository;
            this.storageFactory = storageFactory;
            this.agentFactory = agentFactory;
            this.consumerHubService = consumerHubService;
        }

        public override Task OnConnectedAsync()
        {
            Storage storageToRegister;
            string clientConnectionId = Context.ConnectionId;
            var headers = Context.GetHttpContext().Request.Headers;

            // Check if this storage is already connected.
            string storageName = headers["x-andyx-storage-name"].ToString();
            string agentId = headers["x-andyx-storage-agent-id"].ToString();

            string username = headers["x-andyx-storage-username"].ToString();
            string password = headers["x-andyx-storage-password"].ToString();

            logger.LogInformation($"Storage '{storageName}' with agent id '{agentId}' requested connection");

            if (username != credentialsConfiguration.Username || password != credentialsConfiguration.Password)
            {
                logger.LogInformation($"Storage '{storageName}' with agent id '{agentId}' can not connect, credentials are not valid");
                return base.OnDisconnectedAsync(new Exception($"Check username and password, can not connect to this node"));
            }


            if (storageHubRepository.GetStorageByName(storageName) == null)
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
            if (storageToRegister.Agents.Count - 1 == storageToRegister.AgentMaxNumber)
                return base.OnDisconnectedAsync(new Exception($"There are '{storageToRegister.AgentMaxNumber}' agents connected, connection refused"));

            Agent agentToRegister = agentFactory.CreateAgent(agentId, clientConnectionId, $"{storageName}-{Guid.NewGuid()}");
            storageHubRepository.AddAgent(storageName, clientConnectionId, agentToRegister);

            Clients.Caller.StorageConnected(new Model.Storages.Events.Agents.AgentConnectedDetails()
            {
                AgentId = agentToRegister.AgentId,
                Agent = agentToRegister.AgentName,

                // Send online state of tenants to storage.
                Tenants = tenantMemoryRepository.GetTenants()
            });

            logger.LogInformation($"Storage '{storageName}' with agent id '{agentId}' is connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientConnectionId = Context.ConnectionId;
            Agent agentToRemove = storageHubRepository.GetAgentById(clientConnectionId);
            storageHubRepository.RemoveAgent(clientConnectionId);
            logger.LogInformation($"Storage '{agentToRemove.AgentName}' with agent id '{agentToRemove.AgentId}' is disconnected");

            Clients.Caller.StorageDisconnected(new Model.Storages.Events.Agents.AgentDisconnectedDetails()
            {
                AgentId = agentToRemove.AgentId,
                Agent = agentToRemove.AgentName,

                // Send online state of tenants to storage.
                Tenants = tenantMemoryRepository.GetTenants()
            });

            return base.OnDisconnectedAsync(exception);
        }

        public async Task TransmitMessageToThisNodeConsumers(Message messageDetails)
        {
            await consumerHubService.TransmitMessage(messageDetails, true);
        }

        public async Task TransmitMessagesToConsumer(ConsumerMessage message)
        {
            await consumerHubService.TransmitMessageToConsumer(message);
        }


        public async Task CreateTenant(CreateTenantDetails createTenantDetails)
        {
            await consumerHubService.CreateTenantToThisNode(createTenantDetails);
        }

        public async Task CreateTenantToken(CreateTenantTokenDetails createTenantDetails)
        {
            await consumerHubService.CreateTenantTokenToThisNode(createTenantDetails);
        }
        public async Task RevokeTenantToken(RevokeTenantTokenDetails revokeTenantDetails)
        {
            await consumerHubService.RevokeTenantTokenToThisNode(revokeTenantDetails);
        }

        public async Task CreateComponentToken(CreateComponentTokenDetails createComponentTokenDetails)
        {
            await consumerHubService.CreateComponentTokenToThisNode(createComponentTokenDetails);
        }
        public async Task RevokeComponentToken(RevokeComponentTokenDetails revokeComponentTokenDetails)
        {
            await consumerHubService.RevokeComponentTokenToThisNode(revokeComponentTokenDetails);
        }
    }
}
