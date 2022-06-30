using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using Buildersoft.Andy.X.Utility.Extensions.Packs;
using RocksDbSharp;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Services.Data
{
    public class TopicRocksDbReadonlyDataService : ITopicReadonlyDataService<Message>
    {
        private readonly string _tenant;
        private readonly string _product;
        private readonly string _component;
        private readonly string _topic;

        private readonly string _dataPath;
        private readonly string _logPath;

        private readonly StorageConfiguration _storageConfiguration;

        private readonly DbOptions dbOptions;
        private readonly RocksDb rocksDb;

        private long readBytes = 0;
        private long readKeys = 0;

        public TopicRocksDbReadonlyDataService(string tenant, string product, string component, string topic, StorageConfiguration storageConfiguration)
        {
            _tenant = tenant;
            _product = product;
            _component = component;
            _topic = topic;

            _storageConfiguration = storageConfiguration;

            _dataPath = TenantLocations.GetMessageRootDirectory(tenant, product, component, topic);
            _logPath = TenantLocations.GetTopicLogRootDirectory(tenant, product, component, topic);

            // we will add settings here later from StorageConfiguration
            dbOptions = new DbOptions()
                .SetDbLogDir("dev/null")
                .SetCreateIfMissing(true)
                .SetKeepLogFileNum(storageConfiguration.KeepLogFileNumber)
                .SetStatsDumpPeriodSec(storageConfiguration.DumpStatsInSeconds)
                .SetDeleteObsoleteFilesPeriodMicros(storageConfiguration.DeleteObsoleteFilesPeriodMilliseconds)
                .EnableStatistics();

             rocksDb = RocksDb.OpenReadOnly(dbOptions, _dataPath, true);
        }

        public Message Get(long entryId)
        {
            return Get(rocksDb, entryId);
        }

        public (IEnumerable<Message>, long lastEntryInMessages) GetNMessages(int take, long startEntryId)
        {
            using var db = RocksDb.OpenReadOnly(dbOptions, _dataPath, true);
            long currentEntryId = startEntryId;
            var messages = new List<Message>();
            for (int i = 0; i < take; i++)
            {
                currentEntryId++;

                var message = Get(db, currentEntryId);
                if (message == null)
                    break;

                messages.Add(message);
            }

            // Calculate and add the size and bytes that are read
            readKeys = readKeys + GetCurrentReadKeysStatistics();
            readBytes = readBytes + GetCurrentReadBytesStatistics();

            return (messages, currentEntryId);
        }

        public Message GetNext(long currentEntryId)
        {
            long nextEntryId = currentEntryId + 1;
            var nextMessage = Get(rocksDb, nextEntryId);

            return nextMessage;
        }

        public IEnumerable<Message> GetMessages(IEnumerable<long> keys)
        {
            using var db = RocksDb.OpenReadOnly(dbOptions, _dataPath, true);

            var messages = new List<Message>();
            foreach (var key in keys)
            {
                var message = Get(db, key);
                if (message == null)
                    break;

                messages.Add(message);
            }

            // Calculate and add the size and bytes that are read
            readKeys = readKeys + GetCurrentReadKeysStatistics();
            readBytes = readBytes + GetCurrentReadBytesStatistics();

            return (messages);
        }

        #region Statistics
        public long GetReadKeysStatistics()
        {
            return readKeys;
        }

        public long GetReadBytesStatistics()
        {
            return readBytes;
        }

        private long GetCurrentReadKeysStatistics()
        {
            //41-> "rocksdb.number.keys.read COUNT : 0"
            var statistics = dbOptions.GetStatisticsString().Split("\n");
            string readKeys = statistics[41].Split(":")[1].Replace(" ", "");

            return Convert.ToInt64(readKeys);
        }

        private long GetCurrentReadBytesStatistics()
        {
            //44 -> "rocksdb.bytes.read COUNT : 2584"
            var statistics = dbOptions.GetStatisticsString().Split("\n");
            string readBytes = statistics[44].Split(":")[1].Replace(" ", "");

            return Convert.ToInt64(readBytes);
        }
        #endregion

        private Message Get(RocksDb connectedDb, long entryId)
        {
            var entryIdBytes = entryId.ToEntryBytes();

            var messageBytes = connectedDb.Get(entryIdBytes);
            if (messageBytes == null)
                return null;

            var message = messageBytes.ToObject<Message>();

            return message;
        }
    }
}
