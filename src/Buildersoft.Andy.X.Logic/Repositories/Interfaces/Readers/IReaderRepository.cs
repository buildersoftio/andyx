using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Readers
{
    public interface IReaderRepository
    {
        bool Add(Reader reader);
        bool Edit(Reader reader);
        Reader Delete(string readerName);
        Reader Get(string readerName);
        List<Reader> GetAll();
        void MessageSent(string readerId, string messageId, string value = "Sent");
        void MessageAcknowledged(string readerId, string messageId, string value = "Acknowledged");
        ConcurrentDictionary<string, Reader> GetAllReadersConcurrent();
    }
}
