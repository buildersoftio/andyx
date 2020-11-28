using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Readers
{
    public class ReaderMemoryRepository : IReaderRepository
    {
        private readonly ConcurrentDictionary<string, Reader> _readers;

        public ReaderMemoryRepository(ConcurrentDictionary<string, Reader> readers)
        {
            _readers = readers;
        }

        public bool Add(Reader reader)
        {
            return _readers.TryAdd(reader.Name, reader);
        }

        public Reader Delete(string readerName)
        {
            Reader outReader;
            if (_readers.TryRemove(readerName, out outReader))
                return outReader;
            return null;
        }

        public bool Edit(Reader reader)
        {
            return _readers.TryUpdate(reader.Name, reader, reader);
        }

        public Reader Get(string readerName)
        {
            if (_readers.ContainsKey(readerName))
                return _readers[readerName];
            return null;
        }

        public List<Reader> GetAll()
        {
            return _readers.Values.ToList();
        }

        public ConcurrentDictionary<string, Reader> GetAllReadersConcurrent()
        {
            return _readers;
        }

        public void MessageAcknowledged(string readerId, string messageId, string value = "Acknowledged")
        {
            if (_readers.ContainsKey(readerId))
            {
                if (_readers[readerId].Messages.ContainsKey(readerId))
                    _readers[readerId].Messages[readerId] = value;
                else
                    _readers[readerId].Messages.TryAdd(messageId, value);
            }
        }

        public void MessageSent(string readerId, string messageId, string value = "Sent")
        {
            if (_readers.ContainsKey(readerId))
            {
                if (_readers[readerId].Messages.ContainsKey(readerId))
                    _readers[readerId].Messages[readerId] = value;
                else
                    _readers[readerId].Messages.TryAdd(messageId, value);
            }
        }
    }
}
