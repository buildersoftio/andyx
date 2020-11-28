using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.Enums
{
    public enum ReaderTypes
    {
        /// <summary>
        /// Only one reader
        /// </summary>
        Exclusive,
        /// <summary>
        /// One reader with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one reader.
        /// </summary>
        Shared
    }
}
