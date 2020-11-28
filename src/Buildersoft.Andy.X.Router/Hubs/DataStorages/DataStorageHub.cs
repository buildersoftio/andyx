using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.DataStorages;
using Buildersoft.Andy.X.Data.Model.DataStorages.Events;
using Buildersoft.Andy.X.Data.Model.Router.DataStorages;
using Buildersoft.Andy.X.FileConfig.Files;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.DataStorages;
using Buildersoft.Andy.X.Router.Repositories;
using Buildersoft.Andy.X.Router.Repositories.DataStorages;
using Buildersoft.Andy.X.Utilities.Validation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.DataStorage
{
    public class DataStorageHub : Hub<IDataStorageHub>
    {
        private readonly ILogger<DataStorageHub> _logger;
        private readonly DataStorageRepository _dataStorageRepository;
        private readonly StorageMemoryRepository _memoryRepository;

        public DataStorageHub(ILogger<DataStorageHub> logger, IHubRepository<Data.Model.Router.DataStorages.DataStorage> dataStorageRepository, StorageMemoryRepository memoryRepository)
        {
            _logger = logger;
            _dataStorageRepository = dataStorageRepository as DataStorageRepository;

            // TODO... Discus how memoryRepository will be handled in the future.
            _memoryRepository = memoryRepository;
        }

        public override Task OnConnectedAsync()
        {
            Data.Model.Router.DataStorages.DataStorage dataStorage;
            string clientConnectionId = Context.ConnectionId;

            var headers = Context.GetHttpContext().Request.Headers;

            if (HeaderValidations.IsDataStorageHeaderRequestValid(headers) != true)
                return base.OnDisconnectedAsync(new Exception("Please provide data storage details in body of the request."));

            var dataStorageHost = Context.GetHttpContext().Request.Host;
            dataStorage = new Data.Model.Router.DataStorages.DataStorage()
            {
                DataStoregeId = Guid.NewGuid(),
                DataStorageServer = dataStorageHost.Value,
                DataStorageName = headers["x-andy-storage-name"].ToString(),
                DataStorageEnvironment = (DataStorageEnvironment)Enum.Parse(typeof(DataStorageEnvironment), headers["x-andy-storage-environment"].ToString()),
                DataStorageType = (DataStorageType)Enum.Parse(typeof(DataStorageType), headers["x-andy-storage-type"].ToString()),
                DataStorageStatus = (DataStorageStatus)Enum.Parse(typeof(DataStorageStatus), headers["x-andy-storage-status"].ToString())
            };

            var registered = ConfigFile.GetDataStoragesFromConfig().Where(x => x.DataStorageName == dataStorage.DataStorageName).FirstOrDefault();
            if (registered == null)
            {
                Context.Abort();
                return base.OnDisconnectedAsync(new Exception("This DataStorage Server can not be connected."));
            }

            _dataStorageRepository.Add(clientConnectionId, dataStorage);

            Clients.Caller.DataStorageConnected(new DataStorageConnectionDetail()
            {
                Id = dataStorage.DataStoregeId
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _dataStorageRepository.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public void MessageStoredInDataStorage(MessageStoredArgs message)
        {
            // TODO... Impelement
            // This method will be invoked by DataStorages (not backup ones).
            // Here the message will redirect to Readers hub to send to a specific application that is reading on that book.

        }

        public void TenantAcknowledged(TenantAcknowledgedArgs tenant)
        {
            // Tenant is created in DataStorage
            //TODO... Implement
        }

        public void ProductAcknowledged(ProductAcknowledgedArgs product)
        {
            // Product is created in DataStorage
            //TODO... Implement
        }

        public void ComponentAcknowledged(ComponentAcknowledgedArgs component)
        {
            //TODO... Implement
        }

        public void BookAcknowledged(BookAcknowledgedArgs book)
        {
            //TODO... Implement
        }

        public void ReaderAcknowledgedByDataStorage(ReaderStoredArgs reader)
        {
            //TODO... Implement
        }

        public void SendStorageCurrentState(ConcurrentDictionary<string, Tenant> tenants)
        {
            //_memoryRepository.SetTenants(tenants);
            foreach (var tenant in tenants)
            {
                if (_memoryRepository.GetTenants().ContainsKey(tenant.Key))
                {
                    // Check Products
                    foreach (var product in tenant.Value.Products)
                    {
                        if (_memoryRepository.GetProducts(tenant.Key).ContainsKey(product.Key))
                        {
                            // Check Components
                            foreach (var component in product.Value.Components)
                            {
                                if (_memoryRepository.GetComponents(tenant.Key, product.Key).ContainsKey(component.Key))
                                {
                                    // Check Books
                                    foreach (var book in component.Value.Books)
                                    {
                                        if (_memoryRepository.GetBooks(tenant.Key, product.Key, component.Key).ContainsKey(book.Key) != true)
                                            _memoryRepository.GetBooks(tenant.Key, product.Key, component.Key).TryAdd(book.Key, book.Value);
                                    }
                                }
                                else
                                    _memoryRepository.GetComponents(tenant.Key, product.Key).TryAdd(component.Key, component.Value);
                            }
                        }
                        else
                            _memoryRepository.GetProducts(tenant.Key).TryAdd(product.Key, product.Value);
                    }
                }
                else
                {
                    _memoryRepository.GetTenants().TryAdd(tenant.Key, tenant.Value);
                }
            }
        }
    }
}
