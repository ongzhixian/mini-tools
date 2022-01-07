using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniTools.HostApp.Services;

public class ExampleBackgroundService : BackgroundService
{
    private readonly ILogger<ExampleBackgroundService> logger;

    public ExampleBackgroundService(ILogger<ExampleBackgroundService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ExampleBackgroundService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation($"ExampleBackgroundService task doing background work.");

            // CheckConfirmedGracePeriodOrders();

            await Task.Delay(1000, stoppingToken);
        }
    }
}