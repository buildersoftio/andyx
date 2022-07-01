using RocksDbSharp;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Data
{
    public interface ITopicDataService<T>
    {
        void Put(string key, T message);
        void PutAll(WriteBatch writeBatch);


        T Get(long entryId);
        bool TryGet(long entryId, out T message);
        bool TryGetNext(long entryId, out T message);

        T GetNext(long currentEntryId);
        (IEnumerable<T>, long lastEntryInMessages) GetNMessages(int take, long startEntryId);

        IEnumerable<T> GetMessages(IEnumerable<long> keys);

        long GetWriteKeysStatistics();
        long GetWriteBytesStatistics();

        long GetReadKeysStatistics();
        long GetReadBytesStatistics();

        void Delete(long entryId);
    }
}
