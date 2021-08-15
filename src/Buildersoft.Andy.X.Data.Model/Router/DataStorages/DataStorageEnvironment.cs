using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.Router.DataStorages
{
    public enum DataStorageEnvironment
    {
        Production = 1,
        Test = 2,
        Development = 3
    }

    public enum DataStorageType
    {
        Exclusive = 1,
        Shared = 2,
        Backup = 3
    }

    public enum DataStorageStatus
    {
        // TODO... Kqyre me mire qete pjese, mendoj statuset me ndryshe pasi qe jane server.
        Active = 1,
        Inactive = 2,
        Blocked = 3
    }
}
