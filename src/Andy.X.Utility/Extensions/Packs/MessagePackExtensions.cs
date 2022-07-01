using MessagePack;

namespace Buildersoft.Andy.X.Utility.Extensions.Packs
{
    public static class MessagePackExtensions
    {
        public static byte[] ToEntryBytes(this long entryId)
        {
            return MessagePackSerializer.Serialize(entryId.ToString());
        }

        public static T ToObject<T>(this byte[] messageBytes)
        {
            return MessagePackSerializer.Deserialize<T>(messageBytes);
        }
    }
}
