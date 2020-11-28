using Buildersoft.Andy.X.Data.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.DataStorages
{
    public class BookDetail
    {
        public Guid BookId { get; set; }
        public string TenantName { get; set; }
        public string ProductName { get; set; }
        public string ComponentName { get; set; }
        public string BookName { get; set; }
        public DataTypes DataType { get; set; }

        // TODO... Add properties for this Book
    }
}
