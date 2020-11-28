using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Data.Model.Router.Readers;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Repositories.Readers
{
    public class ReaderRepository : IHubRepository<Reader>
    {
        private readonly StorageMemoryRepository _storageMemoryRepository;
        private ConcurrentDictionary<string, Reader> _readers;
        public ReaderRepository(StorageMemoryRepository storageMemoryRepository)
        {
            _readers = new ConcurrentDictionary<string, Reader>();
            _storageMemoryRepository = storageMemoryRepository;
        }
        public void Add(string id, Reader entity)
        {
            if (_readers.TryAdd(id, entity))
            {
                var readerMemory = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(entity.Tenant, entity.Product, entity.Component, entity.Book));
                readerMemory.Add(new Data.Model.Reader()
                {
                    Id = entity.ReaderId,
                    Name = entity.ReaderName,
                    ReaderTypes = entity.ReaderType,
                    ReaderAs = entity.ReaderAs
                });
            }
        }

        public ConcurrentDictionary<string, Reader> GetAll()
        {
            return _readers;
        }

        public Reader GetById(string id)
        {
            if (_readers.ContainsKey(id))
                return _readers[id];

            return null;
        }

        public string GetId(Reader entity)
        {
            return _readers.FirstOrDefault(p => p.Value == entity).Key;
        }

        public bool Remove(string id)
        {
            Reader deletedReader;
            if (_readers.TryRemove(id, out deletedReader))
            {
                var readerMemory = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(deletedReader.Tenant, deletedReader.Product, deletedReader.Component, deletedReader.Book));
                readerMemory.Delete(deletedReader.ReaderName);
                return true;
            }

            return false;
        }

        public IEnumerable<TResult> Select<TResult>(Func<Reader, TResult> selector)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Reader> Where(Func<Reader, bool> clause)
        {
            return _readers.Values.Where(clause);
        }

        public IEnumerable<KeyValuePair<string, Reader>> GetReaderByName(string readerName)
        {
            return _readers.Where(x => x.Value.ReaderName == readerName);
        }

        public IEnumerable<KeyValuePair<string, Reader>> GetReaders(string tenant, string product, string component, string book, ReaderTypes type)
        {
            return _readers.Where(x => x.Value.Tenant == tenant
            && x.Value.Product == product
            && x.Value.Component == component
            && x.Value.Book == book
            && x.Value.ReaderType == type);
        }
       
        public bool IsReaderConnectable(Reader reader)
        {
            if (reader.ReaderType == ReaderTypes.Exclusive)
            {
                var exclusiveRegisteredReaders = _readers.Where(x => x.Value.Tenant == reader.Tenant
                && x.Value.Product == reader.Product
                && x.Value.Component == reader.Component
                && x.Value.Book == reader.Book
                && x.Value.ReaderName == reader.ReaderName
                && x.Value.ReaderType == ReaderTypes.Exclusive);
                if (exclusiveRegisteredReaders.Count() > 0)
                    return false;
            }
            return true;
        }
    }
}
