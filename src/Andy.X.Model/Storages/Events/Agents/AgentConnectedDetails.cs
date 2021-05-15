using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Agents
{
    public class AgentConnectedDetails
    {
        public Guid ConnectionId { get; set; }
        public string Agent { get; set; }
        public Guid AgentId { get; set; }
    }
}
