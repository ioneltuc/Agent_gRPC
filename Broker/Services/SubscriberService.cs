using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;

namespace Broker.Services
{
    public class SubscriberService : Subscriber.SubscriberBase
    {
        private readonly IConnectionStorageService _connectionStorageService;

        public SubscriberService(IConnectionStorageService connectionStorageService)
        {
            _connectionStorageService = connectionStorageService;
        }

        public override Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            Console.WriteLine($"New {request.Address} client is trying to subscribe to the {request.Topic} topic");

            try
            {
                var connection = new Connection(request.Topic, request.Address);
                _connectionStorageService.Add(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add the new connection {request.Address} to the {request.Topic} topic." +
                    $"\n{ex.Message}");
            }

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true,
            });
        }
    }
}