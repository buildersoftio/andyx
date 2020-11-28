using Buildersoft.Andy.X.Data.Model;
using Buildersoft.Andy.X.Data.Model.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Logic.Interfaces.Readers
{
    public interface IReaderLogic
    {
        Reader CreateReader(string name, ReaderTypes readerTypes);
        Reader UpdateReader(string name, Reader reader);
        Reader DeleteReader(string readerName);
        Reader GetReader(string name);
        List<Reader> GetAllReaders();
        ConcurrentDictionary<string ,Reader> GetReadersDictionary();
    }
}
