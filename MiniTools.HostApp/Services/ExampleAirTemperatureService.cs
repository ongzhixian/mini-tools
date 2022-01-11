using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;

namespace MiniTools.HostApp.Services;

public class ExampleAirTemperatureService : BackgroundService
{
    private readonly ILogger<ExampleAirTemperatureService> logger;
    private readonly IDataPublisher<AirTemperatureInfo> dataProvider;

    public ExampleAirTemperatureService(
        ILogger<ExampleAirTemperatureService> logger,
        IDataPublisher<AirTemperatureInfo> dataProvider)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.dataProvider = dataProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{taskName} is starting.", nameof(ExampleStructDataGeneratorService));

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("{taskName} publishing data.", nameof(ExampleStructDataGeneratorService));

            await dataProvider.PublishDataAsync(stoppingToken);

            await Task.Delay(5000, stoppingToken);
        }
    }

}