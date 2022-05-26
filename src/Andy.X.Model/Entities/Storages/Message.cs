using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buildersoft.Andy.X.Model.Entities.Storages
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long LedgerId { get; set; }

        public string MessageId { get; set; }

        [Column(TypeName = "json")]
        public string Headers { get; set; }

        public byte[] Payload { get; set; }

        public DateTimeOffset StoredDate { get; set; }
        public DateTimeOffset SentDate { get; set; }
    }
}
