namespace Buildersoft.Andy.X.Model.Clusters
{
    public class NodeClient
    {
        public string NodeId { get; set; }
        public string HostName { get; set; }
        public int ShardId { get; set; }
        public ReplicaTypes ReplicaType { get; set; }


        public NodeClient()
        {
            ReplicaType = ReplicaTypes.Main;
        }
    }
}
