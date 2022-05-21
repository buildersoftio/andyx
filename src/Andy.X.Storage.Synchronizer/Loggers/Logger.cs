using System;

namespace Andy.X.Storage.Synchronizer.Loggers
{
    public static class Logger
    {
        public static void Log(params string[] content)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  andyx  Information      |     storage-sync    {string.Join(" ", content)}");
        }
    }
}
