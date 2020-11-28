using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Data.Model
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public ConcurrentDictionary<string, Product> Products { get; set; }

        public Encryption Encryption { private get; set; }
        public Signature Signature { private get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Tenant()
        {
            Id = Guid.NewGuid();
            Description = "";
            Products = new ConcurrentDictionary<string, Product>();
            Status = true;

            Encryption = new Encryption();
            Signature = new Signature();

            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public static Guid GenerateId()
        {
            return Guid.NewGuid();
        }

        public Encryption GetEncryption()
        {
            return Encryption;
        }

        public Signature GetSignature()
        {
            return Signature;
        }
    }
    public class Signature
    {
        public string DigitalSignature { get; set; }
        public string SecurityKey { get; set; }
    }
    public class Encryption
    {
        public bool EncryptionStatus { get; set; }
        public string EncryptionKey { get; set; }
    }
}
