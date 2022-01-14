using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;
using MiniTools.HostApp.Services;
using MiniTools.Messages.Requests;
using MiniTools.Services;
using System.Security.Cryptography.X509Certificates;

var builder = Host.CreateDefaultBuilder(args);

Console.WriteLine("Configuring application options...");

builder.ConfigureServices((host, services) =>
{
    services.Configure<AzureStorageSetting>(host.Configuration.GetSection("Azure:Storage:minitools"));
    services.Configure<AzureWebPubSubSetting>(host.Configuration.GetSection("Azure:WebPubSub:minitools"));

#pragma warning disable S125 // Sections of code should not be commented out

    //services.AddHostedService<ExampleBackgroundService>();

    //services.AddHostedService<QueuePublisherService>();
    //services.AddHostedService<QueueConsumerService>();
    //services.AddHostedService<PubSubConsumerService>();
    //services.AddHostedService<PubSubPublisherService>();

    //services.AddHostedService<StorageTableService>();

#pragma warning restore S125 // Sections of code should not be commented out

    //services.AddHostedService<ExampleBackgroundService>();

    // Channels

    //services.AddSingleton<IChannelQueueService, ChannelQueueService>();
    // --Or the below if we want a more limited queue
    //services.AddSingleton<IChannelQueueService>(_ =>
    //{
    //    if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
    //    {
    //        queueCapacity = 100;
    //    }
    //    return new ChannelQueueService(queueCapacity);
    //});

    //services.AddHostedService<ChannelQueueWorkerService>();
    //var queue = host.Services.GetRequiredService<IChannelQueueService>();
    //queue.EnqueueAsync(stopToken =>
    //{
    //    return new ValueTask(Task.Run(() =>
    //    {
    //        Console.WriteLine("DO SOMETING");
    //    }));
    //});

    // Observers

    // Segregating subscription
    // We have a single subscription object
    // Subscribers subscribes to subscription
    // Providers provide data to subscribers on subscription.
    //services.AddSingleton<DataSubscription<WeatherForecast>>();
    //services.AddSingleton<IStructDataPublisher<WeatherForecast>, WeatherForecastProvider>();

    // Air Temperature Info
    services.AddSingleton<DataSubscription<AirTemperatureInfo>>();
    services.AddSingleton<IDataPublisher<AirTemperatureInfo>, AirTemperatureInfoProvider>();

    // Observer - Observee
    // Observers (subscribers) should always be placed before observees (data providers)
    // (to prevent data from being sent out before subscriptions are in place)
    //services.AddHostedService<ExampleStructDataObserver>()
    //services.AddHostedService<ExampleStructDataGeneratorService>()

    services.AddHostedService<ExampleAirTemperatureObserver>();
    //services.AddHostedService<ExampleAirTemperatureService>();


    // Sets runtime of hosted service via `RUNTIME_SERVICE` environment variable.
    //string runtimeService = Environment.GetEnvironmentVariable("RUNTIME_SERVICE") ?? "MiniTools.HostApp.Services.ExampleBackgroundService";
    //services.AddSingleton<IHostedService>(sp =>
    //{
    //    Type runtimeServiceType = Type.GetType(runtimeService) ?? throw new InvalidOperationException($"runtimeServiceType [{runtimeService}] not found.");
    //    return (IHostedService)ActivatorUtilities.CreateInstance(sp, runtimeServiceType);
    //});

    services.AddHttpClient<UserAuthenticationApiService>();

    //services.AddHostedService<ExampleGrpcService>()
    //services.AddHostedService<ExampleSignalRService>();
    //services.AddHostedService<ExampleBearerService>();

    services.AddGrpcClient<GreetService.GreetServiceClient>("greetService", options =>
    {
        options.Address = new Uri("https://localhost:7001");
    })
    //.ConfigureChannel(o =>
    //{
    //    string _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGV2IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbIkFkbWluaXN0cmF0b3IiLCJEZXZlbG9wZXIiLCJNeVByb2ZpbGUiXSwiZXhwIjoxNjQyMDUwNTg4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDAxLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMDEvIn0.R7EXROt638gOmLVfzxCiIvVUpHDGCvzC9p2IDlq4vPs";

    //    var credentials = CallCredentials.FromInterceptor((context, metadata) =>
    //    {
    //        if (!string.IsNullOrEmpty(_token))
    //        {
    //            metadata.Add("Authorization", $"Bearer {_token}");
    //        }
    //        return Task.CompletedTask;
    //    });

    //    o.Credentials = ChannelCredentials.Create(new SslCredentials(), credentials);
    //})
    ;

    

});

Console.WriteLine("Buiding application host...");

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Running application...");

//using var channel = GrpcChannel.ForAddress("https://localhost:7001");
//var client = new GreetService.GreetServiceClient(channel);
//var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
//Console.WriteLine(reply);

//using (var call = client.StreamingFromClient())
//{
//    for (int i = 0; i < 5; i++)
//    {
//        await call.RequestStream.WriteAsync(new HelloRequest
//        {
//            Name = "Message i " + i.ToString()
//        });
//    }
//    await call.RequestStream.CompleteAsync();
//    var summary = await call.ResponseAsync;
//    Console.WriteLine(summary);
//}

//OrToolService.Example();
//OrToolService.ExampleKnapsack();
//OrToolService.ExampleMultipleKnapsack();

//InferenceService.TrueSkillExample();
//InferenceService.TwoCoinsExample();
//InferenceService.GaussianExample();

//InferenceService.StringExample();
//InferenceService.GaussianArrayExample2();
//InferenceService.BayesPointExample();

InferenceService.BayesSelectionExample();

host.Run();