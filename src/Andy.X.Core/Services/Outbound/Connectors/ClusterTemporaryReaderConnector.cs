using System;
using System.Threading.Tasks;
using System.Timers;

namespace Buildersoft.Andy.X.Core.Services.Outbound.Connectors
{
    public class ClusterTemporaryReaderConnector
    {
        public delegate void StoringCurrentPositionHandler(object sender, string nodeId);
        public event StoringCurrentPositionHandler StoringCurrentPosition;

        public delegate void ReadMessagesFromTempStorageHandler(object sender, string nodeId);
        public event ReadMessagesFromTempStorageHandler ReadMessagesFromTempStorage;

        private readonly Timer currentPositionTimer;
        private readonly Timer readingMessagesTimer;

        private readonly string _nodeId;

        public bool IsReading { get; set; }

        public ClusterTemporaryReaderConnector(string nodeId, int flushCurrentPositionTimer, int backgroundIntervalReadMessages)
        {
            _nodeId = nodeId;

            currentPositionTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 0, 0, flushCurrentPositionTimer).TotalMilliseconds };
            currentPositionTimer.Elapsed += CurrentPositionTimer_Elapsed;

            readingMessagesTimer = new Timer() { AutoReset = true, Interval = new TimeSpan(0, 0, 0, 0, backgroundIntervalReadMessages).TotalMilliseconds };
            readingMessagesTimer.Elapsed += ReadingMessagesTimer_Elapsed;

            IsReading = false;
        }

        public void StartService()
        {
            if (IsReading != true)
            {
                IsReading = true;
                //currentPositionTimer.Start();
                readingMessagesTimer.Start();
            }
        }

        public void StopService()
        {
            if (IsReading == true)
            {
                IsReading = false;
                //currentPositionTimer.Stop();
                readingMessagesTimer.Stop();
            }
        }

        private void ReadingMessagesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            readingMessagesTimer.Stop();
            ReadMessagesFromTempStorage?.Invoke(this, _nodeId);
            readingMessagesTimer.Start();
        }

        private void CurrentPositionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentPositionTimer.Stop();
            StoringCurrentPosition?.Invoke(this, _nodeId);
            currentPositionTimer.Start();
        }
    }
}
