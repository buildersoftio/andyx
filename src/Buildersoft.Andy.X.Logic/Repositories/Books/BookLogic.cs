using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Logic.Interfaces.Books;
using Buildersoft.Andy.X.Utilities.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Logic.Books
{
    public class BookLogic : IBookLogic
    {
        private readonly IBookRepository _bookRepository;
        public BookLogic(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public BookLogic(ConcurrentDictionary<string, Book> books)
        {
            _bookRepository = new BookMemoryRepository(books);
        }

        public Book CreateBook(string name, DataTypes dataTypes, string schemaRawData)
        {
            Book book = new Book()
            {
                InstanceName = name,
                DataType = dataTypes,
                Schema = new Schema()
                {
                    Name = $"{name}-schema",
                    SchemaRawData = schemaRawData,
                }
            };

            if (schemaRawData != "" && schemaRawData != "{}")
            {
                book.Schema.SchemaValidationStatus = true;
            }

            if (_bookRepository.Add(book))
                return book;

            return null;
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public Book GetBook(string name)
        {
            return _bookRepository.Get(name);
        }

        public Schema GetBookSchema(string bookName)
        {
            if (_bookRepository.Get(bookName) != null)
                return _bookRepository.Get(bookName).Schema;

            return null;
        }

        public bool IfSchemaIsDifferent(string bookName, string jsonSchema)
        {
            Schema currentSchema = _bookRepository.Get(bookName).Schema;
            if (currentSchema.SchemaRawData == jsonSchema)
                return false;

            return true;
        }

        public async Task<bool> IsSchemaValidAsync(string bookName, string message)
        {
            Book book = _bookRepository.Get(bookName);
            if (book.Schema.SchemaValidationStatus != true)
                return true;

            return await message.IsSchemaValidAsync(book.Schema.SchemaRawData);
        }

        public Book UpdateBookSchema(string bookName, string jsonSchema, bool isSchemaValid)
        {
            Book book = _bookRepository.Get(bookName);

            Schema currentSchema = book.Schema;
            if (currentSchema.SchemaRawData == jsonSchema && currentSchema.SchemaValidationStatus == isSchemaValid)
                return book;

            book.Schema.Version += 1;
            book.Schema.SchemaRawData = jsonSchema;
            book.Schema.SchemaValidationStatus = isSchemaValid;

            book.Schema.ModifiedDate = DateTime.Now;

            return book;
        }
    }
}
