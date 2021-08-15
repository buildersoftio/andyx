using Buildersoft.Andy.X.Data.Model.DataStorages;
using Buildersoft.Andy.X.Data.Model.Readers.Events;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.Router.Hubs.DataStorage;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.DataStorages;
using Buildersoft.Andy.X.Router.Repositories;
using Buildersoft.Andy.X.Router.Repositories.DataStorages;
using Buildersoft.Andy.X.Utilities.Random;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.DataStorages
{
    public class MessageService
    {
        private readonly IHubContext<DataStorageHub, IDataStorageHub> _hub;
        private readonly DataStorageRepository _dataStorageRepository;

        public MessageService(IHubContext<DataStorageHub, IDataStorageHub> hub, IHubRepository<DataStorage> dataStorageRepository)
        {
            _hub = hub;
            _dataStorageRepository = dataStorageRepository as DataStorageRepository;
        }

        public async Task StoreMessageAsync(MessageDetail messageDetail)
        {
            foreach (var item in Enum.GetNames(typeof(DataStorageEnvironment)))
            {
                DataStorageEnvironment dse = (DataStorageEnvironment)Enum.Parse(typeof(DataStorageEnvironment), item);

                var dataStoragesProductionExclusive = _dataStorageRepository.GetDataStorages(dse,
                    DataStorageType.Exclusive,
                    DataStorageStatus.Active);

                if (dataStoragesProductionExclusive.Count() > 0)
                {
                    string dataStorageConnectionId = dataStoragesProductionExclusive.First().Key;
                    await _hub.Clients.Client(dataStorageConnectionId).MessageStored(messageDetail);

                    // Create a method to send this message to a backup DataStorage

                    break;
                }

                var dataStoragesProductionShared = _dataStorageRepository.GetDataStorages(dse,
                   DataStorageType.Shared,
                   DataStorageStatus.Active);

                if (dataStoragesProductionShared.Count() > 0)
                {
                    int randomSharedDataStorage = RandomGenerator.GetRandomSharedReader(0, dataStoragesProductionShared.Count());

                    string dataStorageConnectionId = dataStoragesProductionShared.ToList()[randomSharedDataStorage].Key;

                    await _hub.Clients.Client(dataStorageConnectionId).MessageStored(messageDetail);

                    // Create a method to send this message to a backup DataStorage

                    break;
                }
            }
        }

        public async Task StoreMessageLogedAsync(MessageLogedArgs messageAcknowledgedArgs)
        {
            //This implementation should change, the acked message should go to all, but only to the datastorage where the message is stored should take it.
            foreach (var item in Enum.GetNames(typeof(DataStorageEnvironment)))
            {
                DataStorageEnvironment dse = (DataStorageEnvironment)Enum.Parse(typeof(DataStorageEnvironment), item);

                var dataStoragesProductionExclusive = _dataStorageRepository.GetDataStorages(dse,
                    DataStorageType.Exclusive,
                    DataStorageStatus.Active);

                if (dataStoragesProductionExclusive.Count() > 0)
                {
                    string dataStorageConnectionId = dataStoragesProductionExclusive.First().Key;
                    await _hub.Clients.Client(dataStorageConnectionId).MessageLogStored(messageAcknowledgedArgs);

                    // Create a method to send this message to a backup DataStorage

                    break;
                }

                var dataStoragesProductionShared = _dataStorageRepository.GetDataStorages(dse,
                   DataStorageType.Shared,
                   DataStorageStatus.Active);

                if (dataStoragesProductionShared.Count() > 0)
                {
                    int randomSharedDataStorage = RandomGenerator.GetRandomSharedReader(0, dataStoragesProductionShared.Count());

                    string dataStorageConnectionId = dataStoragesProductionShared.ToList()[randomSharedDataStorage].Key;

                    await _hub.Clients.Client(dataStorageConnectionId).MessageLogStored(messageAcknowledgedArgs);

                    // Create a method to send this message to a backup DataStorage

                    break;
                }
            }

        }
    }
}
