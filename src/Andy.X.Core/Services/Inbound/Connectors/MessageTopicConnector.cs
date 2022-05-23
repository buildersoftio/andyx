using Buildersoft.Andy.X.Core.Threading;
using Buildersoft.Andy.X.Model.App.Messages;
using System.Collections.Concurrent;

namespace Buildersoft.Andy.X.Core.Services.Inbound.Connectors
{
    public class MessageTopicConnector
    {
        public ThreadPool ThreadingPool { get; set; }
        public ConcurrentQueue<Message> MessagesBuffer { get; set; }

        private readonly int _threadsCount;

        public MessageTopicConnector(int threadsCount)
        {
            _threadsCount = threadsCount;
            MessagesBuffer = new ConcurrentQueue<Message>();
            ThreadingPool = new ThreadPool(threadsCount);

        }
    }
}
