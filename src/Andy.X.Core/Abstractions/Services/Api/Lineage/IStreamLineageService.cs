using Buildersoft.Andy.X.Model.App.Lineage;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Api.Lineage
{
    public interface IStreamLineageService
    {
        List<StreamLineage> GetStreamLineages(string tenant);
        List<StreamLineage> GetStreamLineages(string tenant, string product);
        List<StreamLineage> GetStreamLineages(string tenant, string product, string component);
        StreamLineage GetStreamLineage(string tenant, string product, string component, string topic);
    }
}
