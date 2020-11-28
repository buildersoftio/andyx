using Buildersoft.Andy.X.Data.Model.Router.Streams;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Streams
{
    public class VideoHub : Hub
    {
        public async Task NewViewer(string viewerName)
        {
            var viewerInfo = new Viewer() { ViewerName = viewerName, ConnectionId = Context.ConnectionId };
            await Clients.Others.SendAsync("NewViewerArrived", JsonSerializer.Serialize(viewerInfo));
        }
    }
}
