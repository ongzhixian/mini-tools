using Grpc.Core;
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
    private readonly Metadata headers;

    public ExampleGrpcService(ILogger<ExampleGrpcService> logger, GrpcClientFactory grpcClientFactory)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        client = grpcClientFactory.CreateClient<GreetService.GreetServiceClient>("greetService");

        headers = new Metadata();
        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGV2IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbIkFkbWluaXN0cmF0b3IiLCJEZXZlbG9wZXIiLCJNeVByb2ZpbGUiXSwiZXhwIjoxNjQyMDUwNTg4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDAxLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMDEvIn0.R7EXROt638gOmLVfzxCiIvVUpHDGCvzC9p2IDlq4vPs";
        headers.Add("Authorization", $"Bearer {token}");

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var call = client.StreamingFromClient(headers, null, stoppingToken))
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