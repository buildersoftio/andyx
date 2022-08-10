using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Andy.X.Cluster.Sync.Providers
{
    public class NodeConnectionProvider
    {
        private readonly Replica _replica;
        private readonly ClusterConfiguration _clusterConfiguration;
        private HubConnection? _connection;

        public NodeConnectionProvider(Replica replica,
            ClusterConfiguration clusterConfiguration)
        {
            _replica = replica;
            _clusterConfiguration = clusterConfiguration;

            ConnectToNode(_replica);
        }

        private void ConnectToNode(Replica replica)
        {
            BuildConnection(replica);
        }

        private void BuildConnection(Replica replica)
        {
            string nodeServiceUrl = CreateNodeEndpoint(replica);
            _connection = new HubConnectionBuilder()
                .AddMessagePackProtocol()
                .WithUrl(nodeServiceUrl, option =>
                {
                    // here we gooo...!
                    if (replica.ConnectionType == Buildersoft.Andy.X.Model.NodeConnectionType.NON_SSL)
                    {
                        // skip ssl
                        option.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler httpClientHandler)
                                httpClientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };
                    }
                    else
                    {
                        option.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler httpClientHandler)
                            {
                                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                                httpClientHandler.SslProtocols = SslProtocols.Tls12;
                                var certLocation = Path.Combine(replica.X509CertificateFile);
                                httpClientHandler.ClientCertificates.Add(new X509Certificate2(certLocation, replica.X509CertificateFile));
                            }
                            return message;
                        };
                    }

                    string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(replica.Username + ":" + replica.Password));

                    // Add headers.
                    option.Headers["x-andyx-cluster-authoriziation"] = encoded;

                    option.Headers["x-andyx-cluster-id"] = _clusterConfiguration.Name;
                    option.Headers["x-andyx-node-id"] = replica.NodeId;
                    option.Headers["x-andyx-hostname"] = replica.NodeId;
                    option.Headers["x-andyx-shard-id"] = "-1";
                    option.Headers["x-andyx-replica-type"] = replica.Type.ToString();
                })
                .WithAutomaticReconnect()
                .Build();
        }

        private string CreateNodeEndpoint(Replica replica)
        {
            string endpoint = "http://";
            if (replica.ConnectionType == Buildersoft.Andy.X.Model.NodeConnectionType.SSL)
                endpoint = "https://";

            endpoint += $":{replica.Port}/realtime/v3/cluster";

            return endpoint;
        }

        public HubConnection GetHubConnection()
        {
            return _connection!;
        }
    }
}
