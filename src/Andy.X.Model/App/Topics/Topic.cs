using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Model.App.Topics
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
        public Topic()
        {
            Schema = new Schema();
        }
    }

    public class Schema
    {
        public string Name { get; set; }
        public bool IsSchemaValidated { get; set; }
        public string SchemaRaw { get; set; }
        public int Version { get; set; }

        public Schema()
        {
            Version = 0;
            IsSchemaValidated = false;
        }
    }
}
