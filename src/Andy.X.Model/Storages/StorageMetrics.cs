namespace Buildersoft.Andy.X.Model.Storages
{
    public class StorageMetrics
    {
        public long InRate { get; set; }
        public long OutRate { get; set; }

        public long InThroughput { get; set; }
        public long OutThroughput { get; set; }

        public StorageMetrics()
        {
            InRate = 0;
            OutRate = 0;

            InThroughput = 0;
            OutThroughput = 0;
        }
    }
}
