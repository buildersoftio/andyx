﻿using System.Text.Json.Serialization;

namespace Buildersoft.Andy.X.Model.Configurations
{
    public class StorageConfiguration
    {
        public ulong KeepLogFileNumber { get; set; }
        public uint DumpStatsInSeconds { get; set; }

        // check what is this one? Mili to Micro
        public ulong DeleteObsoleteFilesPeriodMilliseconds { get; set; }

        public bool EnableWriteThreadAdaptiveYield { get; set; }
        public int MaxFileOpeningThreads { get; set; }


        public int InboundMemoryReleaseInMilliseconds { get; set; }
        public int InboundFlushCurrentEntryPositionInMilliseconds { get; set; }

        public int OutboundFlushCurrentEntryPositionInMilliseconds { get; set; }
        public int OutboundBackgroundIntervalReadMessagesInMilliseconds { get; set; }


        // Flushing options
        // write_buffer_size sets the size of a single memtable. Once memtable exceeds this size, it is marked immutable and a new one is created, for now we are creating as 100MB SIZE
        public ulong WriteBufferSizeInBytes { get; set; }
        //max_write_buffer_number sets the maximum number of memtables, both active and immutable. If the active memtable fills up and the total number of memtables is larger than max_write_buffer_number we stall further writes. This may happen if the flush process is slower than the write rate.
        public int MaxWriteBufferNumber { get; set; }

        public int MaxWriteBufferSizeToMaintain { get; set; }

        //min_write_buffer_number_to_merge is the minimum number of memtables to be merged before flushing to storage. For example, if this option is set to 2, immutable memtables are only flushed when there are two of them - a single immutable memtable will never be flushed. If multiple memtables are merged together, less data may be written to storage since two updates are merged to a single key. However, every Get() must traverse all immutable memtables linearly to check if the key is there. Setting this option too high may hurt read performance.
        public int MinWriteBufferNumberToMerge { get; set; }

        public int MaxBackgroundCompactionsThreads { get; set; }
        public int MaxBackgroundFlushesThreads { get; set; }


        public StorageConfiguration()
        {
            DeleteObsoleteFilesPeriodMilliseconds = 30 * 1000 * 60;

            EnableWriteThreadAdaptiveYield = true;
            MaxFileOpeningThreads = 4;

            KeepLogFileNumber = 5;
            DumpStatsInSeconds = 60;

            InboundMemoryReleaseInMilliseconds = 5 * 1000 * 60;
            InboundFlushCurrentEntryPositionInMilliseconds = 5 * 1000;

            OutboundFlushCurrentEntryPositionInMilliseconds = 5 * 1000;
            OutboundBackgroundIntervalReadMessagesInMilliseconds = 500;

            //64MB
            WriteBufferSizeInBytes = 64000000;
            MaxWriteBufferNumber = 4;
            MaxWriteBufferSizeToMaintain = 0;
            MinWriteBufferNumberToMerge = 2;

            MaxBackgroundCompactionsThreads = 1;
            MaxBackgroundFlushesThreads = 1;
        }
    }
}
