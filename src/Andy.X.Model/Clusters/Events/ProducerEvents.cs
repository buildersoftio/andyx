using Buildersoft.Andy.X.Model.Entities.Core.Producers;

namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class ProducerConnectedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public string Producer { get; set; }
        public string ProducerConnectionId { get; set; }
    }

    public class ProducerCreatedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public Producer Producer { get; set; }
    }

    public class ProducerDeletedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public string Producer { get; set; }
    }

    public class ProducerDisconnectedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public string ProducerConnectionId { get; set; }
    }
}
