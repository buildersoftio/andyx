using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Logic.Interfaces.Books;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Books
{
    public class BookMemoryRepository : IBookRepository
    {
        private readonly ConcurrentDictionary<string, Book> _books;
        public BookMemoryRepository(ConcurrentDictionary<string, Book> books)
        {
            _books = books;
        }

        public bool Add(Book queue)
        {
            return _books.TryAdd(queue.InstanceName, queue);
        }

        public Book Get(string bookName)
        {
            if (_books.ContainsKey(bookName))
                return _books[bookName];
            return null;
        }

        public List<Book> GetAll()
        {
            return _books.Values.ToList();
        }

        public ConcurrentDictionary<string, Book> GetBooksConcurrent()
        {
            return _books;
        }
    }
}
