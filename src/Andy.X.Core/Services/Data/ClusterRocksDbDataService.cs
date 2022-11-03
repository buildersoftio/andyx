using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Clusters;
using Buildersoft.Andy.X.Utility.Extensions.Packs;
using MessagePack;
using Microsoft.Extensions.Logging;
using RocksDbSharp;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Services.Data
{
    public class ClusterRocksDbDataService : ITopicDataService<ClusterChangeLog>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly Replica _replica;
        private readonly StorageConfiguration _storageConfiguration;
        private readonly string _dataPath;

        private readonly OptionsHandle dbOptions;
        private readonly RocksDb rocksDb;

        public ClusterRocksDbDataService(
            ILoggerFactory loggerFactory,
            Replica replica,
            StorageConfiguration storageConfiguration)
        {
            _loggerFactory = loggerFactory;
            _replica = replica;
            _storageConfiguration = storageConfiguration;

            _dataPath = ClusterLocations.GetTempClusterMsgsDirectory(_replica.NodeId);

            dbOptions = new DbOptions()
                // Commented for now, custom db log dir.
                //.SetDbLogDir(_logPath)
                .SetCreateIfMissing(true)
                .SetMaxLogFileSize(storageConfiguration.MaxLogFileSizeInBytes)
                .SetStatsDumpPeriodSec(storageConfiguration.DumpStatsInSeconds)
                .SetKeepLogFileNum(storageConfiguration.KeepLogFileNumber)
                .SetMaxBackgroundCompactions(storageConfiguration.ClusterMaxBackgroundCompactionsThreads)
                .SetMaxBackgroundFlushes(storageConfiguration.ClusterMaxBackgroundFlushesThreads)
                .SetWriteBufferSize(storageConfiguration.ClusterWriteBufferSizeInBytes)
                .SetMaxWriteBufferNumber(storageConfiguration.ClusterMaxWriteBufferNumber)
                .SetMaxWriteBufferNumberToMaintain(storageConfiguration.ClusterMaxWriteBufferSizeToMaintain)
                .SetMinWriteBufferNumberToMerge(storageConfiguration.ClusterMinWriteBufferNumberToMerge);

            rocksDb = RocksDb.Open(dbOptions, _dataPath);
        }


        public ClusterChangeLog Get(long entryId)
        {
            var entryIdBytes = entryId.ToEntryBytes();

            var messageBytes = rocksDb.Get(entryIdBytes);
            if (messageBytes == null)
                return null;

            var message = messageBytes.ToObject<ClusterChangeLog>();

            return message;
        }

        public IEnumerable<ClusterChangeLog> GetMessages(IEnumerable<long> keys)
        {
            var messages = new List<ClusterChangeLog>();
            foreach (var key in keys)
            {
                var message = Get(key);
                if (message == null)
                    break;

                messages.Add(message);
            }

            return messages;
        }

        public ClusterChangeLog GetNext(long currentEntryId)
        {
            long nextEntryId = currentEntryId + 1;
            return Get(nextEntryId);
        }

        public (IEnumerable<ClusterChangeLog>, long lastEntryInMessages) GetNMessages(int take, long startEntryId)
        {
            long currentEntryId = startEntryId;
            var messages = new List<ClusterChangeLog>();
            for (int i = 0; i < take; i++)
            {
                currentEntryId++;

                var message = Get(currentEntryId);
                if (message == null)
                    break;

                messages.Add(message);
            }

            return (messages, currentEntryId);
        }

        public void Put(string key, ClusterChangeLog message)
        {
            rocksDb.Put(MessagePackSerializer.Serialize(key), MessagePackSerializer.Serialize(message));
        }

        public void PutAll(WriteBatch writeBatch)
        {
            rocksDb.Write(writeBatch);
        }

        public void Delete(long entryId)
        {
            var entryIdBytes = entryId.ToEntryBytes();
            rocksDb.Remove(entryIdBytes);
        }

        public bool TryGet(long entryId, out ClusterChangeLog message)
        {
            message = Get(entryId);
            if (message == null)
                return false;

            return true;
        }

        public bool TryGetNext(long entryId, out ClusterChangeLog message)
        {
            var nextId = entryId + 1;
            return TryGet(nextId, out message);
        }

        public long GetReadBytesStatistics()
        {
            throw new NotImplementedException();
        }

        public long GetReadKeysStatistics()
        {
            throw new NotImplementedException();
        }

        public long GetWriteBytesStatistics()
        {
            throw new NotImplementedException();
        }

        public long GetWriteKeysStatistics()
        {
            throw new NotImplementedException();
        }

    }
}
