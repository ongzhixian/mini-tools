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

    // services.AddHostedService<ExampleBackgroundService>();

    //services.AddHostedService<QueuePublisherService>();
    //services.AddHostedService<QueueConsumerService>();
    
    services.AddHostedService<PubSubConsumerService>();

    services.AddHostedService<PubSubPublisherService>();

});

Console.WriteLine("Buiding application host...");

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Running application...");

host.Run();