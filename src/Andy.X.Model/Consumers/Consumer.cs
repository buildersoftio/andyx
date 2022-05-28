using System;

namespace Buildersoft.Andy.X.Model.Consumers
{
    public class Consumer
    {
        // check if we really need subscription name here...
        public string Subscription { get; set; }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public ConsumerConnection Connection { get; set; }

        public DateTime ConnectedDate { get; set; }

        public Consumer()
        {
            ConnectedDate = DateTime.Now;
        }
    }

    public class ConsumerConnection
    {
        public string ClientIpAddress { get; set; }
        public string ServerIpAddress { get; set; }
    }
}
