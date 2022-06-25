using Buildersoft.Andy.X.Core.Abstractions.Services.Data;
using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.Configurations;
using Buildersoft.Andy.X.Model.Entities.Storages;
using MessagePack;
using RocksDbSharp;

namespace Buildersoft.Andy.X.Core.Services.Data
{
    public class TopicRocksDbDataService : ITopicDataService
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

        public TopicRocksDbDataService(string tenant, string product, string component, string topic, StorageConfiguration storageConfiguration)
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
                .SetDbLogDir(_logPath)
                .SetCreateIfMissing(true)
                .EnableStatistics();

            rocksDb = RocksDb.Open(dbOptions, _dataPath);
        }

        public void Put(string key, Message message)
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
    }
}
