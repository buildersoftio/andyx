using System;
using System.Diagnostics;

namespace Andy.X.Storage.Synchronizer.Loggers
{
    public static class Logger
    {
        public static void Log(params string[] content)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  andyx   Information      |     storage-sync       process_id={Process.GetCurrentProcess().Id}  {string.Join(" ", content)}");
        }
    }
}
