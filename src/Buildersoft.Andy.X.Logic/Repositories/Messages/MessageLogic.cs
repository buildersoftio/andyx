using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Messages;
using Buildersoft.Andy.X.Utilities.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Buildersoft.Andy.X.Logic.Messages
{
    public class MessageLogic : IMessageLogic
    {
        private readonly IMessageRepository _messageRepository;
        public MessageLogic(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public MessageLogic(ConcurrentDictionary<string, Message> messages)
        {
            _messageRepository = new MessageMemoryRepository(messages);
        }

        public Guid AddMessage(object data)
        {
            Message message = new Message() { Data = data.ToJson() };
            if (_messageRepository.Add(message))
                return message.Id;
            return Guid.Empty;
        }

        public Guid AddMessage(Guid msgId, object data)
        {
            Message message = new Message() { Data = data.ToJson() };
            message.SetId(msgId);
            if (_messageRepository.Add(message))
                return message.Id;
            return Guid.Empty;
        }

        public Guid AddMessageAsBytes(byte[] data)
        {
            Message message = new Message() { DataAsBytes = data };
            if (_messageRepository.Add(message))
                return message.Id;
            return Guid.Empty;
        }

        public List<Message> GetAllMessages()
        {
            return _messageRepository.GetAll();
        }

        public Message GetMessage(Guid id)
        {
            return _messageRepository.Get(id);
        }
    }
}
