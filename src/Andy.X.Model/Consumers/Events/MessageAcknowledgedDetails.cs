using Buildersoft.Andy.X.Model.App.Messages;
using MessagePack;

namespace Buildersoft.Andy.X.Model.Consumers.Events
{
    [MessagePackObject]
    public class MessageAcknowledgedDetails
    {
        [Key(0)]
        public long LedgerId { get; set; }
        [Key(1)]
        public long EntryId { get; set; }

        [Key(2)]
        public int Acknowledgement { get; set; }
    }
}
