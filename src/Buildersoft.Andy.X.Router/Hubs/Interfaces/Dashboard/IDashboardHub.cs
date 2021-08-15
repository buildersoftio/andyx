using Buildersoft.Andy.X.Data.Model.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Interfaces.Dashboard
{
    public interface IDashboardHub
    {
        Task DashboardUserConnected(Object detail);
        Task DashboardUserDisconnected(Object details);

        Task SummaryReportReceived(SummaryReport summaryReport);
    }
}
