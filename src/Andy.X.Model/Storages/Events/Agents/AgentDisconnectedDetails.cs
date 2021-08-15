using Buildersoft.Andy.X.Model.App.Tenants;
using System;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Model.Storages.Events.Agents
{
    public class AgentDisconnectedDetails
    {
        public string Agent { get; set; }
        public Guid AgentId { get; set; }

        public ConcurrentDictionary<string, Tenant> Tenants { get; set; }
    }
}
