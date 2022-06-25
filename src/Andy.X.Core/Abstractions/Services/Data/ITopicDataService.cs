using Buildersoft.Andy.X.Model.Entities.Storages;
using RocksDbSharp;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Data
{
    public interface ITopicDataService
    {
        void Put(string key, Message message);
        void PutAll(WriteBatch writeBatch);
    }
}
