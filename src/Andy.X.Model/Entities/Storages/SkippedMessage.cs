﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildersoft.Andy.X.Model.Entities.Storages
{
    public class SkippedMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Subscription { get; set; }

        public long LedgerId { get; set; }
        public long EntryId { get; set; }


        public DateTimeOffset CreatedDate { get; set; }

        public SkippedMessage()
        {
            CreatedDate = DateTimeOffset.UtcNow;
        }
    }
}
