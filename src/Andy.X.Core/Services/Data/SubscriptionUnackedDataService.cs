using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Utility.Extensions.Packs;
using MessagePack;
using RocksDbSharp;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Services.Data
{
    public class SubscriptionUnackedDataService : ITopicDataService<UnacknowledgedMessage>
    {
        private readonly string _tenant;
        private readonly string _product;
        private readonly string _component;
        private readonly string _topic;
        private readonly string _subscription;

        private readonly string _dataPath;
        private readonly string _logPath;

        private readonly StorageConfiguration _storageConfiguration;

        private readonly DbOptions dbOptions;
        private readonly RocksDb rocksDb;

        public SubscriptionUnackedDataService(string tenant, string product, string component, string topic, string subscription, StorageConfiguration storageConfiguration)
        {
            _tenant = tenant;
            _product = product;
            _component = component;
            _topic = topic;
            _subscription = subscription;

            _storageConfiguration = storageConfiguration;

            _dataPath = TenantLocations.GetSubscriptionUnackedDirectory(tenant, product, component, topic, subscription);
            _logPath = TenantLocations.GetSubscriptionLogsDirectory(tenant, product, component, topic, subscription);

            // we will add settings here later from StorageConfiguration
            dbOptions = new DbOptions()
                //.SetDbLogDir(_logPath)
                .SetCreateIfMissing(true)
                .SetKeepLogFileNum(storageConfiguration.KeepLogFileNumber)
                .SetStatsDumpPeriodSec(storageConfiguration.DumpStatsInSeconds)
                .SetDeleteObsoleteFilesPeriodMicros(storageConfiguration.DeleteObsoleteFilesPeriodMilliseconds)
                .EnableStatistics();

            rocksDb = RocksDb.Open(dbOptions, _dataPath);
        }

        public void Put(string key, UnacknowledgedMessage message)
        {
            rocksDb.Put(MessagePackSerializer.Serialize(key), MessagePackSerializer.Serialize(message));
        }

        public void PutAll(WriteBatch writeBatch)
        {
            using (var db = RocksDb.Open(dbOptions, _dataPath))
            {
                db.Write(writeBatch);
            }
        }

        public long GetWriteBytesStatistics()
        {
            //43 -> "rocksdb.bytes.written COUNT : 2584"
            var statistics = dbOptions.GetStatisticsString().Split("\n");
            string writtenBytes = statistics[43].Split(":")[1].Replace(" ", "");

            return Convert.ToInt64(writtenBytes);
        }

        public long GetWriteKeysStatistics()
        {
            //40 -> "rocksdb.number.keys.written COUNT : 8"
            var statistics = dbOptions.GetStatisticsString().Split("\n");
            string writtenKeys = statistics[40].Split(":")[1].Replace(" ", "");

            return Convert.ToInt64(writtenKeys);
        }

        public UnacknowledgedMessage Get(long entryId)
        {
            var entryIdBytes = entryId.ToEntryBytes();

            var messageBytes = rocksDb.Get(entryIdBytes);
            if (messageBytes == null)
                return null;

            var message = messageBytes.ToObject<UnacknowledgedMessage>();

            return message;
        }

        public UnacknowledgedMessage GetNext(long currentEntryId)
        {
            var nextEntryId = currentEntryId + 1;
            return Get(nextEntryId);
        }

        public (IEnumerable<UnacknowledgedMessage>, long lastEntryInMessages) GetNMessages(int take, long startEntryId)
        {
            long currentEntryId = startEntryId;
            var messages = new List<UnacknowledgedMessage>();
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

        public IEnumerable<UnacknowledgedMessage> GetMessages(IEnumerable<long> keys)
        {
            var messages = new List<UnacknowledgedMessage>();
            foreach (var key in keys)
            {
                var message = Get(key);
                if (message == null)
                    break;

                messages.Add(message);
            }

            return (messages);
        }

        public long GetReadKeysStatistics()
        {
            //41-> "rocksdb.number.keys.read COUNT : 0"
            var statistics = dbOptions.GetStatisticsString().Split("\n");
            string readKeys = statistics[41].Split(":")[1].Replace(" ", "");

            return Convert.ToInt64(readKeys);
        }

        public long GetReadBytesStatistics()
        {
            //44 -> "rocksdb.bytes.read COUNT : 2584"
            var statistics = dbOptions.GetStatisticsString().Split("\n");
            string readBytes = statistics[44].Split(":")[1].Replace(" ", "");

            return Convert.ToInt64(readBytes);
        }

        public bool TryGet(long entryId, out UnacknowledgedMessage message)
        {
            message = Get(entryId);
            if (message == null)
                return false;

            return true;
        }

        public bool TryGetNext(long entryId, out UnacknowledgedMessage message)
        {
            var nextId = entryId + 1;
            return TryGet(nextId, out message);
        }

        public void Delete(long entryId)
        {
            var entryIdBytes = entryId.ToEntryBytes();
            rocksDb.Remove(entryIdBytes);
        }
    }
}
