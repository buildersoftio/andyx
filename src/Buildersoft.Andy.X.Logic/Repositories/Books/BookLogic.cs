using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using Buildersoft.Andy.X.Logic.Interfaces.Books;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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

        public Book CreateBook(string name, DataTypes dataTypes)
        {
            Book queue = new Book() { InstanceName = name, DataType = dataTypes };
            if (_bookRepository.Add(queue))
                return queue;
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
    }
}
