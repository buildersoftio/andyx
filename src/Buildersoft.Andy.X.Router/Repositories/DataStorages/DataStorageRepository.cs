using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Repositories.DataStorages
{
    public class DataStorageRepository : IHubRepository<DataStorage>
    {
        private ConcurrentDictionary<string, DataStorage> _dataStorages;
        public DataStorageRepository()
        {
            _dataStorages = new ConcurrentDictionary<string, DataStorage>();
        }

        public void Add(string id, DataStorage entity)
        {
            _dataStorages.TryAdd(id, entity);
        }

        public ConcurrentDictionary<string, DataStorage> GetAll()
        {
            return _dataStorages;
        }

        public DataStorage GetById(string id)
        {
            if (_dataStorages.ContainsKey(id))
                return _dataStorages[id];

            return null;
        }

        public string GetId(DataStorage entity)
        {
            return _dataStorages.FirstOrDefault(p => p.Value == entity).Key;
        }

        public bool Remove(string id)
        {
            return _dataStorages.TryRemove(id, out _);
        }

        public IEnumerable<TResult> Select<TResult>(Func<DataStorage, TResult> selector)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataStorage> Where(Func<DataStorage, bool> clause)
        {
            return _dataStorages.Values.Where(clause);
        }

        public IEnumerable<KeyValuePair<string, DataStorage>> GetDataStorages(DataStorageEnvironment dataStorageEnvironment, DataStorageType dataStorageType, DataStorageStatus dataStorageStatus)
        {
            return _dataStorages.Where(x => x.Value.DataStorageStatus == dataStorageStatus
               && x.Value.DataStorageEnvironment == dataStorageEnvironment
               && x.Value.DataStorageType == dataStorageType);
        }
    }
}
