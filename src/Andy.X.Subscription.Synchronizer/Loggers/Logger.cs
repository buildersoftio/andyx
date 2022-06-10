using System;
using System.Diagnostics;

namespace Andy.X.Subscription.Synchronizer.Loggers
{
    public static class Logger
    {
        public static void Log(params string[] content)
        {
            Trace.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  andyx   Information      |     subscription-sync  process_id={Process.GetCurrentProcess().Id}  {string.Join(" ", content)}");
        }
    }
}
