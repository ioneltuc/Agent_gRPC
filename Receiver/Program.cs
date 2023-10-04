using Common;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Receiver.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.WebHost.UseUrls(EndpointsConstants.SubscribersAddress);

var app = builder.Build();

app.MapGrpcService<NotificationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Start();

var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAddress);

var client = new Subscriber.SubscriberClient(channel);

string address = app.Services.GetService<IServer>().Features.Get<IServerAddressesFeature>().Addresses.First();
Console.WriteLine($"Subscriber listening at: {address}");

Console.Write("Enter the topic: ");
string topic = Console.ReadLine().ToLower();

var request = new SubscribeRequest()
{
    Topic = topic,
    Address = address,
};

try
{
    SubscribeReply reply = client.Subscribe(request);
    Console.WriteLine($"Reply: {reply.IsSuccess}");
}
catch (Exception ex)
{
    Console.WriteLine($"Could not subscribe: {ex.Message}");
}

Console.ReadLine();