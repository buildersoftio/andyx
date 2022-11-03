using Buildersoft.Andy.X.Core.Threading;
using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Buildersoft.Andy.X.Core.Services.Inbound.Connectors
{
    public class DataConnector<T>
    {
        public delegate void StoringCurrentEntryPositionHandler(object sender, string topicKey);
        public event StoringCurrentEntryPositionHandler StoringCurrentEntryPosition;

        public ThreadPool MessageStoreThreadingPool { get; set; }
        public ConcurrentQueue<T> MessagesBuffer { get; set; }

        private readonly string _topicKey;
        private readonly int _threadsCount;

        private int currentClusterShardIndex = 0;
        private int countClusterShards = 1;

        private readonly Timer releaseMemoryTimer;
        private readonly Timer currentPositionTimer;

        public DataConnector(string topicKey, int threadsCount, int inboundMemoryReleaseInMillisec, int inboundFlashCurrentEntryPositionInMillisec)
        {
            releaseMemoryTimer = new Timer() { Interval = inboundMemoryReleaseInMillisec, AutoReset = true };
            releaseMemoryTimer.Elapsed += ReleaseMemoryTimer_Elapsed;

            currentPositionTimer = new Timer() { AutoReset = true, Interval = inboundFlashCurrentEntryPositionInMillisec };
            currentPositionTimer.Elapsed += CurrentPositionTimer_Elapsed;


            _topicKey = topicKey;
            _threadsCount = threadsCount;

            MessagesBuffer = new ConcurrentQueue<T>();
            MessageStoreThreadingPool = new ThreadPool(threadsCount);

            releaseMemoryTimer.Start();
            currentPositionTimer.Start();
        }

        private void CurrentPositionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentPositionTimer.Stop();

            StoringCurrentEntryPosition?.Invoke(this, _topicKey);

            currentPositionTimer.Start();
        }

        private void ReleaseMemoryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReleaseMemoryMessagingProcessor();
        }

        private void ReleaseMemoryMessagingProcessor()
        {
            if (MessagesBuffer.Count == 0)
            {
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }

        public int GetCurrentClusterShardIndex()
        {
            return currentClusterShardIndex;
        }

        public int GetNextCurrentClusterShardId()
        {
            currentClusterShardIndex++;
            if (currentClusterShardIndex == countClusterShards)
                currentClusterShardIndex = 0;

            return currentClusterShardIndex;
        }

        public int GetClusterShardCount()
        {
            return countClusterShards;
        }

        public void SetClusterShardCount(int count)
        {
            countClusterShards = count;
        }
    }
}
