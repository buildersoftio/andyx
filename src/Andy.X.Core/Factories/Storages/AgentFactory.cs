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

        public Agent CreateAgent(string connectionId, string agentName)
        {
            return new Agent() { ConnectionId = connectionId, AgentName = agentName };
        }
    }
}
