using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Messages
{
    public interface IMessageRepository
    {
        bool Add(Message message);
        Message Get(Guid messageId);
        KeyValuePair<string, Message> GetFirstMessage();
        List<Message> GetAll();
    }
}
