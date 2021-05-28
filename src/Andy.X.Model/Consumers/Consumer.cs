﻿using System;

namespace Buildersoft.Andy.X.Model.Consumers
{
    public class Consumer
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Topic { get; set; }

        public Guid Id { get; set; }
        public string ConsumerName { get; set; }
        public ConsumerType ConsumerType { get; set; }
    }

    public enum ConsumerType
    {
        Exclusive,
        Shared,
        Failover
    }
}