using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildersoft.Andy.X.Model.Entities.Core.Tenants
{
    public class Tenant
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
