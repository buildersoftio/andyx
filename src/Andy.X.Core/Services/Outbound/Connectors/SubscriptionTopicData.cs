using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using Cortex.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Buildersoft.Andy.X.Core.Services.Outbound.Connectors
{
    public class SubscriptionTopicData
    {
        public delegate void TriggerLogicHandler(object sender, string subscriptionId);
        public event TriggerLogicHandler TriggerLogic;

        public Subscription Subscription { get; set; }
        public SubscriptionPosition CurrentPosition { get; set; }

        public ConcurrentPriorityQueue<string, DateTimeOffset> TemporaryMessageQueue { get; set; }
        public ConcurrentDictionary<string, Message> TemporaryMessages { get; set; }

        public long LastLedgerPositionInQueue { get; set; }
        public long LastEntryPositionInQueue { get; set; }

        public bool IsOutboundServiceRunning { get; set; }

        private readonly Timer outboundSubscriptionServiceTimer;

        public SubscriptionTopicData()
        {
            outboundSubscriptionServiceTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 10).TotalMilliseconds };
            outboundSubscriptionServiceTimer.Elapsed += OutboundSubscriptionServiceTimer_Elapsed;

            CurrentPosition = new SubscriptionPosition();

            TemporaryMessageQueue = new ConcurrentPriorityQueue<string, DateTimeOffset>();
            TemporaryMessages = new ConcurrentDictionary<string, Message>();
        }

        public void StartService()
        {
            if (IsOutboundServiceRunning != true)
            {
                IsOutboundServiceRunning = true;
                outboundSubscriptionServiceTimer.Start();
            }
        }

        public void StopService()
        {
            if (IsOutboundServiceRunning == true)
            {
                IsOutboundServiceRunning = false;
                outboundSubscriptionServiceTimer.Stop();
            }
        }

        private void OutboundSubscriptionServiceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            outboundSubscriptionServiceTimer.Stop();

            var subscriptionId = ConnectorHelper.GetSubcriptionId(Subscription.Tenant, Subscription.Product, Subscription.Component, Subscription.Topic, Subscription.SubscriptionName);
            TriggerLogic?.Invoke(this, subscriptionId);

            outboundSubscriptionServiceTimer.Start();
        }
    }
}
