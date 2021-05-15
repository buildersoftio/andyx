using System;

namespace Buildersoft.Andy.X.Model.Storages.Events.Agents
{
    public class AgentConnectedDetails
    {
        public Guid ConnectionId { get; set; }
        public string Agnet { get; set; }
        public Guid AgnetId { get; set; }
    }
}
