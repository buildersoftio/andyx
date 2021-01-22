using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Books;
using Buildersoft.Andy.X.Logic.Interfaces.Messages;
using Buildersoft.Andy.X.Logic.Messages;
using Buildersoft.Andy.X.Router.Services.DataStorages;
using Buildersoft.Andy.X.Utilities.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("")]
    [ApiController]
    [RequireHttps]
    [Authorize(TenantOnly = true)]
    public class MessagesController : ControllerBase
    {
        private IMessageLogic _messageLogic;
        private readonly StorageMemoryRepository _memoryRepository;
        private readonly MessageService _messageService;
        private readonly Router.Services.Readers.ReaderService _readerService;

        public MessagesController(StorageMemoryRepository memoryRepository, MessageService messageService, Router.Services.Readers.ReaderService readerService)
        {
            _memoryRepository = memoryRepository;
            _messageService = messageService;
            _readerService = readerService;
        }

        [HttpGet("api/v1/tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/messages")]
        public ActionResult<List<Message>> GetAllMessages(string tenantName, string productName, string componentName, string bookName)
        {
            _messageLogic = new MessageLogic(
                _memoryRepository.GetMessages(tenantName, productName, componentName, bookName));

            return Ok(_messageLogic.GetAllMessages());
        }

        [HttpGet("api/v1/tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/messages/{msgId}")]
        public ActionResult<Message> GetMessage(string tenantName, string productName, string componentName, string bookName, Guid msgId)
        {
            _messageLogic = new MessageLogic(
                _memoryRepository.GetMessages(tenantName, productName, componentName, bookName));

            Data.Model.Message message = _messageLogic.GetMessage(msgId);
            if (message != null)
                return Ok(message);

            return NotFound("MESSAGE_NOT_FOUND");
        }

        [HttpPost("api/v1/tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/messages")]
        public async Task<ActionResult<Guid>> CreateMessageAsJsonFromApi(string tenantName, string productName, string componentName, string bookName, [FromBody] object msg)
        {
            Guid generatedMessageId = Guid.NewGuid();
            var bookLogic = new BookLogic(
                _memoryRepository.GetBooks(tenantName, productName, componentName));

            if (await bookLogic.IsSchemaValidAsync(bookName, msg.ToString()) != true)
                return BadRequest("INVALID_SCHEMA");

            //Store the message into DataStorage
            await _messageService.StoreMessageAsync(new Data.Model.DataStorages.MessageDetail()
            {
                MessageId = generatedMessageId.ToString(),
                Tenant = tenantName,
                Product = productName,
                Component = componentName,
                Book = bookName,
                Message = msg
            });

            // This part will be removed and moved to DataStorageHub, will be inoked by MessageStoredInStorage
            await _readerService.SendMessageAsync(new Data.Model.Readers.MessageDetail()
            {
                MessageId = generatedMessageId.ToString(),
                Tenant = tenantName,
                Product = productName,
                Component = componentName,
                Book = bookName,
                Message = msg
            });

            return Ok(generatedMessageId);
        }

        [HttpPost("{tenantName}/{productName}/{componentName}/{bookName}")]
        public async Task<ActionResult<Guid>> CreateMessageAsJson(string tenantName, string productName, string componentName, string bookName, [FromQuery] Guid msgId, [FromBody] object msg)
        {
            Guid generatedMessageId = msgId;
            if (generatedMessageId == Guid.Empty)
                generatedMessageId = Guid.NewGuid();

            var bookLogic = new BookLogic(
                _memoryRepository.GetBooks(tenantName, productName, componentName));

            if (await bookLogic.IsSchemaValidAsync(bookName, msg.ToString()) != true)
                return BadRequest("INVALID_SCHEMA");

            //Store the message into DataStorage
            await _messageService.StoreMessageAsync(new Data.Model.DataStorages.MessageDetail()
            {
                MessageId = generatedMessageId.ToString(),
                Tenant = tenantName,
                Product = productName,
                Component = componentName,
                Book = bookName,
                Message = msg
            });

            // This part will be removed and moved to DataStorageHub, will be inoked by MessageStoredInStorage
            await _readerService.SendMessageAsync(new Data.Model.Readers.MessageDetail()
            {
                MessageId = generatedMessageId.ToString(),
                Tenant = tenantName,
                Product = productName,
                Component = componentName,
                Book = bookName,
                Message = msg
            });

            return Ok(generatedMessageId);
        }
    }
}