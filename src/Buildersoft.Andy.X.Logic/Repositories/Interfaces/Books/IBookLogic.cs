using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Logic.Interfaces.Books
{
    public interface IBookLogic
    {
        Book CreateBook(string name, DataTypes dataTypes, string schemaRawData);
        Book GetBook(string name);
        List<Book> GetAllBooks();
        Schema GetBookSchema(string bookName);
        Book UpdateBookSchema(string bookName, string jsonSchema, bool isSchemaValid);

        bool IfSchemaIsDifferent(string bookName, string jsonSchema);
        Task<bool> IsSchemaValidAsync(string bookName, string message);
    }
}
