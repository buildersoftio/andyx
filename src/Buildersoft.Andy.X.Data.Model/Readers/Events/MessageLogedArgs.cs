using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model.Readers.Events
{
    public class MessageLogedArgs
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Book { get; set; }
        public string Reader { get; set; }
        public string MessageId { get; set; }
        public string Log { get; set; }
        public DateTime Date { get; set; }
    }
}
