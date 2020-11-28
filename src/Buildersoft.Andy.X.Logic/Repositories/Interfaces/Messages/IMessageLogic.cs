using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Messages
{
    public interface IMessageLogic
    {
        Guid AddMessage(object data);
        Guid AddMessage(Guid msgId, object data);
        Guid AddMessageAsBytes(byte[] data);
        Message GetMessage(Guid id);
        List<Message> GetAllMessages();
    }
}
