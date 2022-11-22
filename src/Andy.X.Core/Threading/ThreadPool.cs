using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Buildersoft.Andy.X.Core.Threading
{
    public class ThreadPool
    {
        public ConcurrentDictionary<Guid, ThreadDetail> Threads { get; set; }
        public bool AreThreadsRunning { get; set; }

        public ThreadPool(int size)
        {
            AreThreadsRunning = false;
            Threads = new ConcurrentDictionary<Guid, ThreadDetail>();
            for (int i = 0; i < size; i++)
            {
                var details = new ThreadDetail();
                details.UpdateMainThreadRunningStatus += Details_UpdateMainThreadRunningStatus;
                Threads.TryAdd(Guid.NewGuid(), details);
            }
        }

        private void Details_UpdateMainThreadRunningStatus(bool isThreadWorking)
        {
            // check if all isThreadWorking eq false, update AreThreadsRunning to false;
            if (Threads.Where(x => x.Value.IsThreadWorking == true).Count() == 0)
            {
                AreThreadsRunning = false;
                return;
            }

            if (isThreadWorking == false)
            {
                AreThreadsRunning = false;
            }
        }
    }

   
}
