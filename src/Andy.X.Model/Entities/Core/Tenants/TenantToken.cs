﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildersoft.Andy.X.Model.Entities.Core.Tenants
{
    public class TenantToken
    {
        [ForeignKey("Tenants")]
        public long TenantId { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Secret { get; set; }

        public bool IsActive { get; set; }

        [Column(TypeName = "json")]
        public string Roles { get; set; }

        public DateTimeOffset ExpireDate { get; set; }

        public string Description { get; set; }
        public DateTimeOffset IssuedDate { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        public string CreatedBy { get; set; }
    }

    public enum TenantTokenRole
    {
        Produce,
        Consume
    }

}
