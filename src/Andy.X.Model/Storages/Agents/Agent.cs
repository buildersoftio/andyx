using System;

namespace Buildersoft.Andy.X.Model.Storages.Agents
{
    public class Agent
    {
        public string ConnectionId { get; set; }
        public Guid AgentId { get; set; }

        // Think! I don't know if we need AgentName
        public string AgentName { get; set; }

        public Agent()
        {
            AgentId = Guid.NewGuid();
        }
    }
}
