using Buildersoft.Andy.X.Data.Model.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Repositories.Interfaces.Dashboard
{
    public interface IDashboardSummaryMemoryRepository
    {
        void Update(SummaryReport summaryReport);
        SummaryReport Get();
    }
}
