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
        public delegate void StoringCurrentPositionHandler(object sender, string subscriptionId);
        public event StoringCurrentPositionHandler StoringCurrentPosition;

        public delegate void ReadMessagesFromStorageHandler(object sender, string subscriptionId);
        public event ReadMessagesFromStorageHandler ReadMessagesFromStorage;

        public Subscription Subscription { get; set; }
        public SubscriptionPosition CurrentPosition { get; set; }

        public ConcurrentPriorityQueue<string, DateTimeOffset> TemporaryMessageQueue { get; set; }
        public ConcurrentDictionary<string, Message> TemporaryMessages { get; set; }

        public long LastLedgerPositionInQueue { get; set; }
        public long LastEntryPositionInQueue { get; set; }

        public bool IsConsuming { get; set; }

        public bool IsOutboundServiceRunning { get; set; }


        private readonly Timer currentPositionTimer;
        private readonly Timer readingMessagesTimer;

        public SubscriptionTopicData()
        {
            currentPositionTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 10).TotalMilliseconds };
            currentPositionTimer.Elapsed += CurrentPositionTimer_Elapsed;

            readingMessagesTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 1).TotalMilliseconds };
            readingMessagesTimer.Elapsed += ReadingMessagesTimer_Elapsed;

            CurrentPosition = new SubscriptionPosition();

            TemporaryMessageQueue = new ConcurrentPriorityQueue<string, DateTimeOffset>();
            TemporaryMessages = new ConcurrentDictionary<string, Message>();

            IsConsuming = false;
        }

        public void StartService()
        {
            if (IsOutboundServiceRunning != true)
            {
                IsOutboundServiceRunning = true;
                currentPositionTimer.Start();
                readingMessagesTimer.Start();
            }
        }

        public void StopService()
        {
            if (IsOutboundServiceRunning == true)
            {
                IsOutboundServiceRunning = false;
                currentPositionTimer.Stop();
                IsConsuming = false;
                readingMessagesTimer.Stop();
            }
        }

        public void SetConsumingFlag()
        {
            if (IsConsuming == false)
            {
                IsConsuming = true;
                readingMessagesTimer.Stop();
            }
        }

        public void UnsetConsumingFlag()
        {
            if (IsConsuming == true)
            {
                IsConsuming = false;
                readingMessagesTimer.Start();
            }
        }

        private void CurrentPositionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentPositionTimer.Stop();

            var subscriptionId = ConnectorHelper.GetSubcriptionId(Subscription.Tenant, Subscription.Product, Subscription.Component, Subscription.Topic, Subscription.SubscriptionName);
            StoringCurrentPosition?.Invoke(this, subscriptionId);

            currentPositionTimer.Start();
        }

        private void ReadingMessagesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentPositionTimer.Stop();

            var subscriptionId = ConnectorHelper.GetSubcriptionId(Subscription.Tenant, Subscription.Product, Subscription.Component, Subscription.Topic, Subscription.SubscriptionName);
            ReadMessagesFromStorage?.Invoke(this, subscriptionId);

            currentPositionTimer.Start();
        }
    }
}
