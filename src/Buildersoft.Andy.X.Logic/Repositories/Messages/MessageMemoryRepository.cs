using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Messages
{
    public class MessageMemoryRepository : IMessageRepository
    {
        private readonly ConcurrentDictionary<string, Message> _messages;
        public MessageMemoryRepository(ConcurrentDictionary<string, Message> messages)
        {
            _messages = messages;
        }

        public bool Add(Message message)
        {
            return _messages.TryAdd(message.Id.ToString(), message);
        }

        public Message Get(Guid messageId)
        {
            if (_messages.ContainsKey(messageId.ToString()))
                return _messages[messageId.ToString()];
            return null;
        }

        public List<Message> GetAll()
        {
            return _messages.Values.ToList();
        }

        public KeyValuePair<string, Message> GetFirstMessage()
        {
            return _messages.First();
        }
    }
}
