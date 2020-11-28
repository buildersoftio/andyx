using Buildersoft.Andy.X.Data.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Books
{
    public interface IBookLogic
    {
        Data.Model.Book CreateBook(string name, DataTypes dataTypes);
        Data.Model.Book GetBook(string name);
        List<Data.Model.Book> GetAllBooks();
    }
}
