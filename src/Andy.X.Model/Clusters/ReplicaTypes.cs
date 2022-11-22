namespace Buildersoft.Andy.X.Model.Clusters
{
    public enum ReplicaTypes
    {
        Main,
        Worker,

        // when MainOrWorker is set, one of the Nodes will produce an event to other nodes to tell that who is the main node.
        // we are skipping MainOrWorker for v3.0.0 Release
        //MainOrWorker,

        // In this replica no producer or consumer can connect when the other nodes are working (its perfect for DRC)
        BackupReplica
    }
}
