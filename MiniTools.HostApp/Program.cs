using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;
using MiniTools.HostApp.Services;


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


    services.AddSingleton<IChannelQueueService, ChannelQueueService>();
    services.AddHostedService<ChannelQueueWorkerService>();
    services.AddHostedService<ExampleBackgroundService>();

    //services.AddSingleton<IChannelQueueService>(_ =>
    //{
    //    if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
    //    {
    //        queueCapacity = 100;
    //    }

    //    return new ChannelQueueService(queueCapacity);
    //});

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

var queue = host.Services.GetRequiredService<IChannelQueueService>();
queue.EnqueueAsync(stopToken =>
{
    return new ValueTask(Task.Run(() =>
    {
        Console.WriteLine("DO SOMETING");
    }));
});


host.Run();