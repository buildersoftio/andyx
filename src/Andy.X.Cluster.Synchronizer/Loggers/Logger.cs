using System;

namespace Andy.X.Cluster.Synchronizer.Loggers
{
    public static class Logger
    {
        public static void Log(params string[] content)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  andyx  Information      |     cluster-sync    {string.Join(" ", content)}");
        }
    }
}
