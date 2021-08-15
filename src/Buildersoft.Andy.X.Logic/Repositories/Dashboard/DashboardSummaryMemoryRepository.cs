using Buildersoft.Andy.X.Data.Model.Dashboard;
using Buildersoft.Andy.X.Logic.Repositories.Interfaces.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Repositories.Dashboard
{
    public class DashboardSummaryMemoryRepository : IDashboardSummaryMemoryRepository
    {
        private SummaryReport _summaryReport;
        public DashboardSummaryMemoryRepository()
        {
            _summaryReport = new SummaryReport();
        }

        public SummaryReport Get()
        {
            return _summaryReport;
        }

        public void Update(SummaryReport summaryReport)
        {
            _summaryReport = summaryReport;
        }
    }
}
