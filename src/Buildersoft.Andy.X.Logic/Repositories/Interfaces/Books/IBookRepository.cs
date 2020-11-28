using Buildersoft.Andy.X.Data.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Books
{
    public interface IBookRepository
    {
        bool Add(Book queue);
        Book Get(string instanceName);
        List<Book> GetAll();

        ConcurrentDictionary<string, Book> GetBooksConcurrent();
    }
}
