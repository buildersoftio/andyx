using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.Router.DataStorages
{
    public class DataStorage
    {
        public Guid DataStoregeId { get; set; }
        public string DataStorageName { get; set; }
        public string DataStorageServer { get; set; }
        public DataStorageEnvironment DataStorageEnvironment { get; set; }
        public DataStorageType DataStorageType { get; set; }
        public DataStorageStatus DataStorageStatus { get; set; }
        public DataStorage()
        {
            DataStoregeId = Guid.NewGuid();
        }
    }
}
