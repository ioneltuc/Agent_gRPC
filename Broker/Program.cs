using Broker.Services;
using Broker.Services.Interfaces;
using Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<IMessageStorageService, MessageStorageService>();
builder.Services.AddSingleton<IConnectionStorageService, ConnectionStorageService>();
builder.Services.AddHostedService<SendWorker>();
builder.WebHost.UseUrls(EndpointsConstants.BrokerAddress);

var app = builder.Build();

app.MapGrpcService<PublisherService>();
app.MapGrpcService<SubscriberService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();