using Buildersoft.Andy.X.Model.Storages.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Storages
{
    public interface IAgentFactory
    {
        Agent CreateAgent();
        Agent CreateAgent(string  agentId,string connectionId, string agentName);
    }
}
