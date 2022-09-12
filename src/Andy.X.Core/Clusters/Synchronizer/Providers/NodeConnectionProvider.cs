using Buildersoft.Andy.X.Model.Clusters;
using Buildersoft.Andy.X.Model.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Buildersoft.Andy.X.Core.Clusters.Synchronizer.Providers
{
    public class NodeConnectionProvider
    {
        private readonly Replica _replica;
        private readonly ClusterConfiguration _clusterConfiguration;
        private readonly NodeConfiguration _nodeConfiguration;
        private HubConnection _connection;

        public NodeConnectionProvider(Replica replica,
            ClusterConfiguration clusterConfiguration,
            NodeConfiguration nodeConfiguration)
        {
            _replica = replica;
            _clusterConfiguration = clusterConfiguration;
            _nodeConfiguration = nodeConfiguration;

            ConnectToNode();
        }

        private void ConnectToNode( )
        {
            BuildConnection();
        }

        private void BuildConnection()
        {
            string nodeServiceUrl = CreateNodeEndpoint(_replica);
            _connection = new HubConnectionBuilder()
                .AddMessagePackProtocol()
                .WithUrl(nodeServiceUrl, option =>
                {
                    // here we gooo...!
                    if (_replica.ConnectionType == Buildersoft.Andy.X.Model.NodeConnectionType.NON_SSL)
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
                                var certLocation = Path.Combine(_replica.X509CertificateFile);
                                httpClientHandler.ClientCertificates.Add(new X509Certificate2(certLocation, _replica.X509CertificateFile));
                            }
                            return message;
                        };
                    }

                    string encodedBasicToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(_replica.Username + ":" + _replica.Password));


                    // Add headers..
                    option.Headers.Add("Authorization", "Basic " + encodedBasicToken);
                    option.Headers["x-andyx-cluster-id"] = _clusterConfiguration.Name;
                    option.Headers["x-andyx-node-id"] = _replica.NodeId;
                    option.Headers["x-andyx-node-id-from"] = _nodeConfiguration.NodeId;
                    option.Headers["x-andyx-hostname"] = _replica.Host;
                    option.Headers["x-andyx-shard-id"] = "-1";
                    option.Headers["x-andyx-replica-type"] = _replica.Type.ToString();
                })
                .WithAutomaticReconnect()
                .Build();
        }

        private string CreateNodeEndpoint(Replica replica)
        {
            string endpoint = "http://";
            if (replica.ConnectionType == Buildersoft.Andy.X.Model.NodeConnectionType.SSL)
                endpoint = "https://";

            endpoint += $"{replica.Host}:{replica.Port}/realtime/v3/cluster";

            return endpoint;
        }

        public HubConnection GetHubConnection()
        {
            return _connection!;
        }
    }

}
