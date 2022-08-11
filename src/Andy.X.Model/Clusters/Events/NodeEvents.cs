using MessagePack;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    [MessagePackObject]
    public class NodeConnectedArgs
    {
        [Key(0)]
        public string NodeId { get; set; }
        [Key(1)]
        public string HostName { get; set; }
        [Key(2)]
        public int ShardId { get; set; }
        [Key(3)]
        public string ReplicaType { get; set; }
    }

    [MessagePackObject]
    public class NodeDisconnectedArgs
    {
        [Key(0)]
        public string NodeId { get; set; }
        [Key(1)]
        public string HostName { get; set; }
        [Key(2)]
        public int ShardId { get; set; }
        [Key(3)]
        public string ReplicaType { get; set; }
    }
}
