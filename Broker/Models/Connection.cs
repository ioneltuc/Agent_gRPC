using Grpc.Net.Client;

namespace Broker.Models
{
    public class Connection
    {
        public string Topic { get; }
        public string Address { get; }
        public GrpcChannel GrpcChannel { get; }

        public Connection(string topic, string address)
        {
            Topic = topic;
            Address = address;
            GrpcChannel = GrpcChannel.ForAddress(address);
        }
    }
}