using Broker.Models;
using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;

namespace Broker.Services
{
    public class SendWorker : IHostedService
    {
        private Timer _timer;
        private const int TimeToWait = 1000;
        private readonly IMessageStorageService _messageStorageService;
        private readonly IConnectionStorageService _connectionStorageService;

        public SendWorker(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                _messageStorageService = scope.ServiceProvider.GetRequiredService<IMessageStorageService>();
                _connectionStorageService = scope.ServiceProvider.GetRequiredService<IConnectionStorageService>();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, 0, TimeToWait);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            while (!_messageStorageService.IsEmpty())
            {
                Message message = _messageStorageService.GetNext();

                if (message != null)
                {
                    var connections = _connectionStorageService.GetConnectionsByTopic(message.Topic);

                    foreach (var connection in connections)
                    {
                        var client = new Notifier.NotifierClient(connection.GrpcChannel);
                        var request = new NotifyRequest()
                        {
                            Content = message.Content
                        };

                        try
                        {
                            var reply = client.Notify(request);
                            Console.WriteLine($"Subscriber {connection.Address} has been notified." +
                                $"\nResponse: {reply.IsSuccess}");
                        }
                        catch (RpcException rpcEx)
                        {
                            if (rpcEx.StatusCode == StatusCode.Internal || rpcEx.StatusCode == StatusCode.Unavailable)
                            {
                                _connectionStorageService.Remove(connection.Address);
                            }

                            Console.WriteLine($"Subscriber {connection.Address} could not be notified because of an RPC error." +
                                $"\n{rpcEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Subscriber {connection.Address} could not be notified." +
                                $"\n{ex.Message}");
                        }
                    }
                }
            }
        }
    }
}