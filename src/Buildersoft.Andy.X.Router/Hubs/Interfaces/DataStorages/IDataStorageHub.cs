using Buildersoft.Andy.X.Data.Model.DataStorages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Interfaces.DataStorages
{
    public interface IDataStorageHub
    {
        Task DataStorageConnected(DataStorageConnectionDetail connectionDetail);
        Task DataStorageDisconnected(DataStorageDisconnectionDetail disconnectionDetail);
        Task MessageStored(MessageDetail message);
        Task ReaderStored(ReaderDetail reader);


        Task TenantCreated(TenantDetail tenant);
        Task TenantRead(TenantDetail tenant);
        Task TenantUpdated(TenantDetail tenant);
        Task TenantDeleted(TenantDetail tenant);


        Task ProductCreated(ProductDetail product);
        Task ProductRead(ProductDetail product);
        Task ProductUpdated(ProductDetail product);
        Task ProductDeleted(ProductDetail product);


        Task ComponentCreated(ComponentDetail component);
        Task ComponentRead(ComponentDetail component);
        Task ComponentUpdated(ComponentDetail component);
        Task ComponentDeleted(ComponentDetail component);


        Task BookCreated(BookDetail book);
        Task BookRead(BookDetail book);
        Task BookUpdated(BookDetail book);
        Task BookDeleted(BookDetail book);
    }
}
