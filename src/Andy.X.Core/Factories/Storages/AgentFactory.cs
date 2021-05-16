using Buildersoft.Andy.X.Core.Abstractions.Factories.Storages;
using Buildersoft.Andy.X.Model.Storages.Agents;
using System;

namespace Buildersoft.Andy.X.Core.Factories.Storages
{
    public class AgentFactory : IAgentFactory
    {
        public Agent CreateAgent()
        {
            return new Agent();
        }

        public Agent CreateAgent(string agentId, string connectionId, string agentName)
        {
            return new Agent() { AgentId = Guid.Parse(agentId), ConnectionId = connectionId, AgentName = agentName };
        }
    }
}
