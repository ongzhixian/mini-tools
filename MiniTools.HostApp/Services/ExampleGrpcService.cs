using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.Messages.Requests;
using MiniTools.Services;

namespace MiniTools.HostApp.Services;

public class ExampleGrpcService : BackgroundService
{
    private readonly ILogger<ExampleGrpcService> logger;
    private readonly GreetService.GreetServiceClient client;


    public ExampleGrpcService(ILogger<ExampleGrpcService> logger, GrpcClientFactory grpcClientFactory)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        client = grpcClientFactory.CreateClient<GreetService.GreetServiceClient>("greetService");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var call = client.StreamingFromClient())
            {
                for (int i = 0; i < 10; i++)
                {
                    logger.LogInformation("sending message");
                    await call.RequestStream.WriteAsync(new HelloRequest
                    {
                        Name = "Message i " + i.ToString()
                    });

                    await Task.Delay(1000, stoppingToken);
                }

                //await call.RequestStream.CompleteAsync();
                //var summary = await call.ResponseAsync;
                //Console.WriteLine(summary);
            }
        }
    }
}