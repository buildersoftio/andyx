using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Data.Model.Readers;
using Buildersoft.Andy.X.Data.Model.Router.Readers;
using Buildersoft.Andy.X.Router.Hubs.DataStorage;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.DataStorages;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.Readers;
using Buildersoft.Andy.X.Router.Hubs.Readers;
using Buildersoft.Andy.X.Router.Repositories;
using Buildersoft.Andy.X.Router.Repositories.Readers;
using Buildersoft.Andy.X.Router.Services.DataStorages;
using Buildersoft.Andy.X.Utilities.Random;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Services.Readers
{
    public class ReaderService
    {
        private readonly IHubContext<ReaderHub, IReaderHub> _readerHub;
        private readonly MessageService _messageService;
        private readonly ReaderRepository _readerRepository;
        public ReaderService(IHubContext<ReaderHub, IReaderHub> hub, IHubRepository<Reader> readerRepository, MessageService messageService)
        {
            _readerHub = hub;
            _messageService = messageService;
            _readerRepository = readerRepository as ReaderRepository;
        }

        public async Task SendMessageAsync(MessageDetail message)
        {
            await SendMessageToExclusiveReaders(message);
            await SendMessageToSharedReaders(message);
        }

        private async Task SendMessageToSharedReaders(MessageDetail message)
        {
            var sharedReaders = _readerRepository.GetReaders(message.Tenant, message.Product, message.Component, message.Book, ReaderTypes.Shared);
            if (sharedReaders.Count() > 0)
            {
                int randomReader = RandomGenerator.GetRandomSharedReader(0, sharedReaders.Count());
                string readerConnectionId = sharedReaders.ToList()[randomReader].Key;
                await _readerHub.Clients.Client(readerConnectionId).MessageReceived(message);

                // Store the message in dataStorage with status sent
                await _messageService.StoreMessageLogedAsync(new Data.Model.Readers.Events.MessageLogedArgs()
                {
                    Tenant = message.Tenant,
                    Product = message.Product,
                    Component = message.Component,
                    Book = message.Book,
                    Reader = sharedReaders.ToList()[randomReader].Value.ReaderName,
                    MessageId = message.MessageId,
                    Log = "sent",
                    Date = DateTime.Now
                });
            }
        }

        private async Task SendMessageToExclusiveReaders(MessageDetail message)
        {
            var exclusiveReaders = _readerRepository.GetReaders(message.Tenant, message.Product, message.Component, message.Book, ReaderTypes.Exclusive);
            foreach (var reader in exclusiveReaders)
            {
                await _readerHub.Clients.Client(reader.Key).MessageReceived(message);

                // Store the message in dataStorage with status sent
                await _messageService.StoreMessageLogedAsync(new Data.Model.Readers.Events.MessageLogedArgs()
                {
                    Tenant = message.Tenant,
                    Product = message.Product,
                    Component = message.Component,
                    Book = message.Book,
                    Reader = reader.Value.ReaderName,
                    MessageId = message.MessageId,
                    Log = "sent",
                    Date = DateTime.Now
                });
            }
        }
    }
}
