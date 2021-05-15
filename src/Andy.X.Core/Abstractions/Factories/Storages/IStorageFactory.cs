using Buildersoft.Andy.X.Model.Storages;

namespace Buildersoft.Andy.X.Core.Abstractions.Factories.Storages
{
    public interface IStorageFactory
    {
        Storage CreateStorage();
        Storage CreateStorage(string storageName, StorageStatus storageStatus, bool isLoadBalanced, int agnetMaxNumber, int agnetMinNumber);
    }
}
