﻿using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Clusters
{
    public interface IClusterService
    {
        //TODO: Implement this Service to enable connection between Nodes with each-other, like in the cluster_configuration.json file.
        void LoadClusterConfigurationInMemory(ClusterConfiguration clusterConfiguration);
        void ChangeClusterStatus(ClusterStatus clusterStatus);

        Cluster GetCluster();
    }
}
