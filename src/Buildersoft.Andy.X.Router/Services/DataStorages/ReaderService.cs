using Buildersoft.Andy.X.Data.Model.DataStorages;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.Router.Hubs.DataStorage;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.DataStorages;
using Buildersoft.Andy.X.Router.Repositories;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.DataStorages
{
    public class ReaderService
    {
        private readonly IHubContext<DataStorageHub, IDataStorageHub> _hub;
        private readonly IHubRepository<DataStorage> _dataStorageRepository;

        public ReaderService(IHubContext<DataStorageHub, IDataStorageHub> hub, IHubRepository<DataStorage> dataStorageRepository)
        {
            _hub = hub;
            _dataStorageRepository = dataStorageRepository;
        }

        public async Task StoreReaderAsync(ReaderDetail reader)
        {
            foreach (var storage in _dataStorageRepository.GetAll())
            {
                await _hub.Clients.Client(storage.Key).ReaderStored(reader);
            }
        }
    }
}
