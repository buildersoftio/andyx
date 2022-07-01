using MessagePack;

namespace Buildersoft.Andy.X.Model.Consumers.Events
{
    [MessagePackObject]
    public class MessageAcknowledgedDetails
    {
        [Key(0)]
        public long EntryId { get; set; }

        [Key(1)]
        public int Acknowledgement { get; set; }
    }
}
