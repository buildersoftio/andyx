using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Configurations
{

    public class ClusterConfiguration
    {
        public string Name { get; set; }
        public List<XNode> Nodes { get; set; }
        public ClusterConfiguration()
        {
            Nodes = new List<XNode>();
        }
    }

    public class XNode
    {
        public string NodeId { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NodeConnectionType NodeConnectionType { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public XNode()
        {
            NodeId = "default_01";
            NodeConnectionType = NodeConnectionType.NON_SSL;
        }
    }
}
