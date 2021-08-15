using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.DataStorages
{
    public class DataStorageServer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ServerName { get; set; }
        public int Port { get; set; }
    }
}
