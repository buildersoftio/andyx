using Buildersoft.Andy.X.Data.Model.Router.Dashboard;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.Dashboard;
using Buildersoft.Andy.X.Router.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Dashboard
{ 
    public class DashboardHub : Hub<IDashboardHub>
    {
        private readonly ILogger<DashboardHub> _logger;
        private readonly IHubRepository<DashboardUser> _userRepository;

        public DashboardHub(ILogger<DashboardHub> logger, IHubRepository<DashboardUser> userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public override Task OnConnectedAsync()
        {
            string clientConnectionId = Context.ConnectionId;

            _userRepository.Add(clientConnectionId, new DashboardUser()
            {
                ConnectionId = clientConnectionId
            });

            Clients.Caller.DashboardUserConnected(new
            {
                State = "Connected",
                Date = DateTime.Now
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userRepository.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
