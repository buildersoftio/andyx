using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Threading
{
    public class ThreadDetail
    {
        public delegate void UpdateMainThreadRunningStatusHandler(bool isWorking);
        public event UpdateMainThreadRunningStatusHandler UpdateMainThreadRunningStatus;

        public Task Task { get; set; }

        private bool isThreadWorking = false;
        public bool IsThreadWorking
        {
            get
            {
                return isThreadWorking;
            }
            set
            {
                isThreadWorking = value;
                UpdateMainThreadRunningStatus?.Invoke(value);
            }
        }

        public ThreadDetail()
        {
            Task = null;
        }
    }
}
