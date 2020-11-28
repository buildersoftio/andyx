using Buildersoft.Andy.X.Data.Model.Readers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Router.Hubs.Interfaces.Readers
{
    public interface IReaderHub
    {
        Task ReaderConnected(ReaderConnectionDetail connectionDetail);
        Task ReaderDisconnected(ReaderDisconnectionDetail disconnectionDetail);
        Task MessageReceived(MessageDetail messageDetail);
    }
}
