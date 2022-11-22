using MessagePack;

namespace Buildersoft.Andy.X.Model.Entities.Storages
{
    [MessagePackObject]
    public class UnacknowledgedMessage
    {
        [Key(0)]
        public long MessageEntry { get; set; }
    }
}
