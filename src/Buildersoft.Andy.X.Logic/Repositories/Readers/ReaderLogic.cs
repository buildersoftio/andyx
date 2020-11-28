using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Logic.Interfaces.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Readers
{
    public class ReaderLogic : IReaderLogic
    {
        private readonly IReaderRepository _readerRepository;
        public ReaderLogic(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;
        }

        public ReaderLogic(ConcurrentDictionary<string, Reader> readers)
        {
            _readerRepository = new ReaderMemoryRepository(readers);
        }

        public Reader CreateReader(string name, ReaderTypes readerTypes)
        {
            Reader reader = new Reader() { Name = name, ReaderTypes = readerTypes };
            if (_readerRepository.Add(reader))
                return reader;
            return null;
        }

        public Reader DeleteReader(string readerName)
        {
            Reader reader = _readerRepository.Delete(readerName);
            if (reader != null)
                return reader;
            return null;
        }

        public List<Reader> GetAllReaders()
        {
            return _readerRepository.GetAll();
        }

        public Reader GetReader(string name)
        {
            return _readerRepository.Get(name);
        }

        public ConcurrentDictionary<string, Reader> GetReadersDictionary()
        {
            return _readerRepository.GetAllReadersConcurrent();
        }

        public Reader UpdateReader(string name, Reader reader)
        {
            if (_readerRepository.Edit(reader))
                return reader;
            return null;
        }
    }
}
