using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Data
{
    public interface ITopicReadonlyDataService<T>
    {
        T Get(long entryId);
        T GetNext(long currentEntryId);
        (IEnumerable<T>, long lastEntryInMessages) GetNMessages(int take, long startEntryId);

        IEnumerable<T> GetMessages(IEnumerable<long> keys);

        long GetReadKeysStatistics();
        long GetReadBytesStatistics();
    }
}
