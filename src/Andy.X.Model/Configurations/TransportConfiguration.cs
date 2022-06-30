namespace Buildersoft.Andy.X.Model.Configurations
{
    public class TransportConfiguration
    {
        // The server considers the client disconnected if it hasn't received a message (including keep-alive) in this interval. It could take longer than this timeout interval for the client to be marked disconnected due to how this is implemented. The recommended value is double the KeepAliveInterval value.
        public int ClientTimeoutInterval { get; set; }

        // If the client doesn't send an initial handshake message within this time interval, the connection is closed. This is an advanced setting that should only be modified if handshake timeout errors are occurring due to severe network latency. For more detail on the handshake process
        public int HandshakeTimeout { get; set; }

        // If the server hasn't sent a message within this interval, a ping message is sent automatically to keep the connection open. When changing KeepAliveInterval, change the ServerTimeout or serverTimeoutInMilliseconds setting on the client. The recommended ServerTimeout or serverTimeoutInMilliseconds value is double the KeepAliveInterval value.
        public int KeepAliveInterval { get; set; }

        // The maximum number of items that can be buffered for client upload streams. If this limit is reached, the processing of invocations is blocked until the server processes stream items.
        public int StreamBufferCapacity { get; set; }

        // Maximum size of a single incoming hub message.
        public long? MaximumReceiveMessageSizeInBytes { get; set; }

        // The maximum number of hub methods that each client can call in parallel before queueing.
        public int MaximumParallelInvocationsPerClient { get; set; }

        // The maximum number of bytes received from the client that the server buffers before applying backpressure. Increasing this value allows the server to receive larger messages faster without applying backpressure, but can increase memory consumption.
       
        
        
        public long ApplicationMaxBufferSizeInBytes { get; set; }

        // 	The maximum number of bytes sent by the app that the server buffers before observing backpressure. Increasing this value allows the server to buffer larger messages faster without awaiting backpressure, but can increase memory consumption.
        public long TransportMaxBufferSizeInBytes { get; set; }
    }
}
