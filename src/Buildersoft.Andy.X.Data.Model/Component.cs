using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public ConcurrentDictionary<string, Book> Books { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Component()
        {
            Id = Guid.NewGuid();
            Books = new ConcurrentDictionary<string, Book>();
            Description = "";
            Status = true;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }
        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}
