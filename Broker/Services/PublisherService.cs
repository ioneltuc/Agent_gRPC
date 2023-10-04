using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;

namespace Broker.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private readonly IMessageStorageService _messageStorageService;

        public PublisherService(IMessageStorageService messageStorageService)
        {
            _messageStorageService = messageStorageService;
        }

        public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received: {request.Topic} {request.Content}");

            var message = new Message(request.Topic, request.Content);

            _messageStorageService.Add(message);

            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true,
            });
        }
    }
}