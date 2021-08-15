using Buildersoft.Andy.X.Data.Model.Router.Dashboard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Router.Repositories.Dashboard
{
    public class DashboardUserRepository : IHubRepository<DashboardUser>
    {
        private ConcurrentDictionary<string, DashboardUser> _dashboardUsers;
        public DashboardUserRepository()
        {
            _dashboardUsers = new ConcurrentDictionary<string, DashboardUser>();
        }

        public void Add(string id, DashboardUser entity)
        {
            _dashboardUsers.TryAdd(id, entity);
        }

        public ConcurrentDictionary<string, DashboardUser> GetAll()
        {
            return _dashboardUsers;
        }

        public DashboardUser GetById(string id)
        {
            if (_dashboardUsers.ContainsKey(id))
                return _dashboardUsers[id];

            return null;
        }

        public string GetId(DashboardUser entity)
        {
            return _dashboardUsers.FirstOrDefault(p => p.Value == entity).Key;
        }

        public bool Remove(string id)
        {
            if (_dashboardUsers.TryRemove(id, out _))
                return true;

            return false;
        }

        public IEnumerable<TResult> Select<TResult>(Func<DashboardUser, TResult> selector)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DashboardUser> Where(Func<DashboardUser, bool> clause)
        {
            throw new NotImplementedException();
        }
    }
}
