using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Messages;
using ProtoBuf;
using System.IO;

namespace Buildersoft.Andy.X.IO.Services
{
    public static class MessageIOService
    {
        public static bool TrySaveInTemp_MessageBinFile(Message message, string msgId)
        {
            try
            {
                var msgLocation = TenantLocations.GetNextMessageToStoreFile(message.Tenant, message.Product, message.Component, message.Topic, msgId);
                using (var fs = File.Create(msgLocation))
                {
                    Serializer.Serialize(fs, message);
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
