using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Model.Entities.Subscriptions;
using Buildersoft.Andy.X.Model.Subscriptions;
using Buildersoft.Andy.X.Utility.Extensions.Helpers;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Timers;

namespace Buildersoft.Andy.X.Core.Services.Outbound.Connectors
{
    public class SubscriptionTopicData
    {
        public delegate void StoringCurrentPositionHandler(object sender, string subscriptionId);
        public event StoringCurrentPositionHandler StoringCurrentPosition;

        public delegate Task<bool> ReadMessagesFromStorageHandler(object sender, string subscriptionId);
        public event ReadMessagesFromStorageHandler ReadMessagesFromStorage;

        public Subscription Subscription { get; set; }
        public SubscriptionPosition CurrentPosition { get; set; }

        public TopicEntryPosition TopicState { get; set; }

        public ConcurrentDictionary<string, long> TemporaryUnackedMessageIds { get; set; }

        public bool IsConsuming { get; set; }

        public long LastMessageEntryPositionSent { get; set; }
        public long LastUnackedMessageEntryPositionSent { get; set; }

        public bool IsOutboundServiceRunning { get; set; }


        private readonly Timer currentPositionTimer;
        private readonly Timer readingMessagesTimer;

        public SubscriptionTopicData(int flushCurrentPositionTimer, int backgroundIntervalReadMessages)
        {
            currentPositionTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 0, 0, flushCurrentPositionTimer).TotalMilliseconds };
            currentPositionTimer.Elapsed += CurrentPositionTimer_Elapsed;

            readingMessagesTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 0, 0, backgroundIntervalReadMessages).TotalMilliseconds };
            readingMessagesTimer.Elapsed += ReadingMessagesTimer_Elapsed;

            CurrentPosition = new SubscriptionPosition();

            TemporaryUnackedMessageIds = new ConcurrentDictionary<string, long>();

            LastMessageEntryPositionSent = 0;
            LastUnackedMessageEntryPositionSent = 0;

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

        private async void ReadingMessagesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            readingMessagesTimer.Stop();

            var subscriptionId = ConnectorHelper.GetSubcriptionId(Subscription.Tenant, Subscription.Product, Subscription.Component, Subscription.Topic, Subscription.SubscriptionName);

            // if resut is true, there are consumers connected to this subscription
            var result = await ReadMessagesFromStorage?.Invoke(this, subscriptionId);

            if (result == true)
                readingMessagesTimer.Start();
            else
                readingMessagesTimer.Stop();
        }
    }
}
