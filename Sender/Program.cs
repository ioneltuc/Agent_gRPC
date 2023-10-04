using Common;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAgent;

Console.WriteLine("Publisher");

var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAddress);

var client = new Publisher.PublisherClient(channel);

while (true)
{
    Console.Write("Enter the topic: ");
    string topic = Console.ReadLine().ToLower();

    Console.Write("Enter content: ");
    string content = Console.ReadLine();

    var request = new PublishRequest()
    {
        Topic = topic,
        Content = content
    };

    try
    {
        PublishReply reply = await client.PublishMessageAsync(request);
        Console.WriteLine($"Reply: {reply.IsSuccess}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not publish the message: {ex.Message}");
    }
}