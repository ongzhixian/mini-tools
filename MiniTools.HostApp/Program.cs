using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Services;

// using IHost host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices((_, services) =>
//         services.AddTransient<ITransientOperation, DefaultOperation>()
//             .AddScoped<IScopedOperation, DefaultOperation>()
//             .AddSingleton<ISingletonOperation, DefaultOperation>()
//             .AddTransient<OperationLogger>())
//     .Build();


var builder = Host.CreateDefaultBuilder(args);

Console.WriteLine("Configuring application options...");

builder.ConfigureServices(services =>
{
    services.AddHostedService<ExampleBackgroundService>();
});

Console.WriteLine("Buiding application host...");

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Hello world");

host.Run();