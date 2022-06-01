using Buildersoft.Andy.X.IO.Locations;
using Buildersoft.Andy.X.Model.App.Messages;
using MessagePack;
using System;
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
                    // for now we will not use proto-buf, we will contine to use MessagePack
                    //Serializer.Serialize(fs, message);
                    MessagePackSerializer.Serialize(fs, message);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TrySaveInTemp_UnackedMessageBinFile(MessageAcknowledgementFileContent messageContent, string msgId)
        {
            try
            {
                var msgLocation = TenantLocations.GetNextUnAckedMessageToStoreFile(messageContent.Tenant, messageContent.Product, messageContent.Component, messageContent.Topic, msgId);
                using (var fs = File.Create(msgLocation))
                {
                    // for now we will not use proto-buf, we will contine to use MessagePack
                    MessagePackSerializer.Serialize(fs, messageContent);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Message ReadMessage_FromBinFile(string binLocation)
        {
            try
            {
                return MessagePackSerializer.Deserialize<Message>(File.ReadAllBytes(binLocation));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static MessageAcknowledgementFileContent ReadAckedMessage_FromBinFile(string binLocation)
        {
            try
            {
                return MessagePackSerializer.Deserialize<MessageAcknowledgementFileContent>(File.ReadAllBytes(binLocation));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
