using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;

namespace MiniTools.HostApp.Services;

public class ExampleStructDataGeneratorService : BackgroundService
{
    private readonly ILogger<ExampleStructDataGeneratorService> logger;
    private readonly IStructDataPublisher<WeatherForecast> dataProvider;

    public ExampleStructDataGeneratorService(
        ILogger<ExampleStructDataGeneratorService> logger,
        IStructDataPublisher<WeatherForecast> dataProvider)
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