using Buildersoft.Andy.X.Data.Model.Enums;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model
{
    public class Book
    {
        public Guid Id { get; set; }
        public string InstanceName { get; set; }
        public ConcurrentDictionary<string, Message> Messages { get; set; }
        public ConcurrentDictionary<string, Reader> Readers { get; set; }

        public Schema Schema { get; set; }

        public DataTypes DataType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Book()
        {
            Id = Guid.NewGuid();
            Messages = new ConcurrentDictionary<string, Message>();
            Readers = new ConcurrentDictionary<string, Reader>();

            Schema = new Schema();

            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }

    public class Schema
    {
        public string Name { get; set; }

        // Check if the schema should be validated before messages are transmitting via Andy.
        public bool SchemaValidationStatus { get; set; }

        public string SchemaRawData { get; set; }
        public int Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Schema()
        {
            SchemaValidationStatus = false;
            Version = 1;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }
    }
}
