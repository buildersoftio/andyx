using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Data.Model.Readers.Events;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Readers;
using Buildersoft.Andy.X.Router.Hubs.Interfaces.Readers;
using Buildersoft.Andy.X.Router.Repositories;
using Buildersoft.Andy.X.Router.Repositories.Readers;
using Buildersoft.Andy.X.Utilities.Attributes;
using Buildersoft.Andy.X.Utilities.Validation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Readers
{
    [Authorize(TenantOnly = true)]
    public class ReaderHub : Hub<IReaderHub>
    {
        private readonly ILogger<ReaderHub> _logger;
        private readonly StorageMemoryRepository _storageMemoryRepository;
        private readonly Services.DataStorages.ReaderService _readerService;
        private readonly ReaderRepository _readerRepository;

        public ReaderHub(ILogger<ReaderHub> logger,
            IHubRepository<Data.Model.Router.Readers.Reader> readerRepository,
            StorageMemoryRepository storageMemoryRepository,
            Services.DataStorages.ReaderService readerService)
        {
            _logger = logger;
            _storageMemoryRepository = storageMemoryRepository;
            _readerService = readerService;
            _readerRepository = readerRepository as ReaderRepository;
        }

        public override Task OnConnectedAsync()
        {
            Data.Model.Router.Readers.Reader reader;
            string clientConnectionId = Context.ConnectionId;

            var headers = Context.GetHttpContext().Request.Headers;

            if (HeaderValidations.IsReaderHeaderRequestValid(headers) != true)
                return base.OnDisconnectedAsync(new Exception("Please provide reader details in header of the request. Check the documentation at Buildersoft Online"));

            reader = new Data.Model.Router.Readers.Reader()
            {
                ReaderId = Guid.NewGuid(),
                Tenant = headers["x-andy-x-tenant"].ToString(),
                Product = headers["x-andy-x-product"].ToString(),
                Component = headers["x-andy-x-component"].ToString(),
                Book = headers["x-andy-x-book"].ToString(),
                ReaderName = headers["x-andy-x-reader"].ToString(),
                ReaderType = (ReaderTypes)Enum.Parse(typeof(ReaderTypes), headers["x-andy-x-readertype"].ToString()),
                ReaderAs = (ReaderAs)Enum.Parse(typeof(ReaderAs), headers["x-andy-x-readeras"].ToString())
            };

            if (_readerRepository.IsReaderConnectable(reader) != true)
            {
                Context.Abort();
                return base.OnDisconnectedAsync(new Exception($"This reader is already connected to this book {reader.Book}, please check the dashboard."));
            }

            _readerRepository.Add(clientConnectionId, reader);

            _ = _readerService.StoreReaderAsync(new Data.Model.DataStorages.ReaderDetail()
            {
                Tenant = reader.Tenant,
                Product = reader.Product,
                Component = reader.Component,
                Book = reader.Book,
                ReaderAs = reader.ReaderAs,
                ReaderType = reader.ReaderType,
                ReaderName = reader.ReaderName
            });

            Clients.Caller.ReaderConnected(new Data.Model.Readers.ReaderConnectionDetail()
            {
                ReaderId = reader.ReaderId
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _readerRepository.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }


        public void AcknowledgeMessage(MessageAcknowledgedArgs messageAcked)
        {
            var readerRepository = new ReaderMemoryRepository(_storageMemoryRepository.GetReaders(messageAcked.Tenant, messageAcked.Product, messageAcked.Component, messageAcked.Book));
            readerRepository.MessageAcknowledged(messageAcked.Reader, messageAcked.MessageId);
        }
    }
}
