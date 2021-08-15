using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Repositories
{
    public interface IHubRepository<T> where T : class
    {
        T GetById(string id);
        ConcurrentDictionary<string, T> GetAll();
        string GetId(T entity);
        void Add(string id, T entity);
        bool Remove(string id);

        IEnumerable<T> Where(Func<T, bool> clause);
        IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector);
    }
}
