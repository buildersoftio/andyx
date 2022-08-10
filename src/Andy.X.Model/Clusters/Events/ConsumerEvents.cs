namespace Buildersoft.Andy.X.Model.Clusters.Events
{
    public class ConsumerConnectedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public string Subscription { get; set; }
        public string Consumer { get; set; }
        public string ConsumerConnectionId { get; set; }
    }

    public class ConsumerDisconnectedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }
        public string Subscription { get; set; }
        public string ConsumerConnectionId { get; set; }
    }
}
