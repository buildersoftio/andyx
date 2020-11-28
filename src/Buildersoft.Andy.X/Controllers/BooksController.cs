using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Logic;
using Buildersoft.Andy.X.Logic.Books;
using Buildersoft.Andy.X.Logic.Interfaces.Books;
using Buildersoft.Andy.X.Router.Services.DataStorages;
using Buildersoft.Andy.X.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildersoft.Andy.X.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [RequireHttps]
    [Authorize(TenantOnly = true)]
    public class BooksController : ControllerBase
    {
        private IBookLogic _bookLogic;
        private readonly StorageMemoryRepository _memoryRepository;
        private readonly BookService _bookService;
        public BooksController(StorageMemoryRepository memoryRepository, BookService bookService)
        {
            _memoryRepository = memoryRepository;
            _bookService = bookService;
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books")]
        public ActionResult<List<Book>> GetAllBooks(string tenantName, string productName, string componentName)
        {
            _bookLogic = new BookLogic(
                _memoryRepository.GetBooks(tenantName, productName, componentName));

            return Ok(_bookLogic.GetAllBooks());
        }

        [HttpGet("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}")]
        public ActionResult<Book> GetBook(string tenantName, string productName, string componentName, string bookName)
        {
            _bookLogic = new BookLogic(
                _memoryRepository.GetBooks(tenantName, productName, componentName));

            Book book = _bookLogic.GetBook(bookName);
            if (book != null)
                return Ok(book);

            return NotFound("BOOK_NOT_FOUND");
        }

        [HttpPost("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}")]
        public async Task<ActionResult<Book>> CreateBook(string tenantName, string productName, string componentName, string bookName)
        {
            _bookLogic = new BookLogic(
                _memoryRepository.GetBooks(tenantName, productName, componentName));

            if (_bookLogic.GetBook(bookName) != null)
                return BadRequest("BOOK_EXISTS");

            Book book = _bookLogic.CreateBook(bookName, DataTypes.Json);
            if (book != null)
            {
                await _bookService.CreateBookAsync(new Data.Model.DataStorages.BookDetail()
                {
                    BookId = book.Id,
                    TenantName = tenantName,
                    ProductName = productName,
                    ComponentName = componentName,
                    BookName = bookName,
                    DataType = DataTypes.Json
                });

                return Ok(book);
            }

            return BadRequest("BOOK_NOT_CREATED");
        }

        [HttpPost("tenants/{tenantName}/products/{productName}/components/{componentName}/books/{bookName}/type/{dataType}")]
        public async Task<ActionResult<Book>> CreateBook(string tenantName, string productName, string componentName, string bookName, string dataType)
        {
            _bookLogic = new BookLogic(
                _memoryRepository.GetBooks(tenantName, productName, componentName));

            if (_bookLogic.GetBook(bookName) != null)
                return BadRequest("BOOK_EXISTS");
            DataTypes _dataType = (DataTypes)Enum.Parse(typeof(DataTypes), dataType);
            Book book = _bookLogic.CreateBook(bookName, _dataType);
            if (book != null)
            {
                await _bookService.CreateBookAsync(new Data.Model.DataStorages.BookDetail()
                {
                    BookId = book.Id,
                    TenantName = tenantName,
                    ProductName = productName,
                    ComponentName = componentName,
                    BookName = bookName,
                    DataType = _dataType
                });

                return Ok(book);
            }

            return BadRequest("BOOK_NOT_CREATED");
        }
    }
}