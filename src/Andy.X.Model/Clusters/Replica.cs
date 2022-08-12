using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Buildersoft.Andy.X.Model.Clusters
{

    public class Replica
    {
        public string NodeId { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NodeConnectionType ConnectionType { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReplicaTypes Type { get; set; }

        public bool IsConnected { get; set; }
        public bool IsLocal { get; set; }


        //X509 Certs
        public string X509CertificateFile { get; set; }
        public string X509CertificatePassword { get; set; }


        public Replica()
        {
            NodeId = "standalone_01";
            Type = ReplicaTypes.MainOrWorker;
            ConnectionType = NodeConnectionType.NON_SSL;

            IsConnected = false;
            IsLocal = false;
        }
    }

    public class ReplicaShardConnection
    {
        public string NodeId { get; set; }
        public string NodeConnectionId { get; set; }
    }
}
