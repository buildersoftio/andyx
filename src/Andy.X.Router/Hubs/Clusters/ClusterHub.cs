using Buildersoft.Andy.X.Core.Abstractions.Hubs.Clusters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Clusters
{
    public class ClusterHub : Hub<IClusterHub>
    {
        private readonly ILogger<ClusterHub> _logger;

        public ClusterHub(ILogger<ClusterHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
