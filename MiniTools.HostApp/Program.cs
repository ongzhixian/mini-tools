using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;
using MiniTools.HostApp.Services;
using MiniTools.Messages.Requests;
using MiniTools.Services;

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

});

Console.WriteLine("Buiding application host...");

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Running application...");

using var channel = GrpcChannel.ForAddress("https://localhost:7001");

var client = new GreetService.GreetServiceClient(channel);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine(reply);

using (var call = client.StreamingFromClient())
{
    for (int i = 0; i < 5; i++)
    {
        await call.RequestStream.WriteAsync(new HelloRequest
        {
            Name = "Message i " + i.ToString()
        });
    }

    await call.RequestStream.CompleteAsync();

    var summary = await call.ResponseAsync;

    Console.WriteLine(summary);
}

host.Run();