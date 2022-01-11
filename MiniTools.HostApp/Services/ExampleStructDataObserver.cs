using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;

namespace MiniTools.HostApp.Services;

public class ExampleStructDataObserver : BackgroundService
{
    private readonly ILogger<ExampleStructDataObserver> logger;
    private readonly DataSubscription<WeatherForecast> dataSubscription;
    private readonly IStructDataSubscriber<WeatherForecast> dataSubscriber;

    public ExampleStructDataObserver(ILogger<ExampleStructDataObserver> logger,
        DataSubscription<WeatherForecast> dataSubscription)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.dataSubscription = dataSubscription;

        // Subscriber should be a class that acts on the data received through the subscription
        // In this example, WeatherForecastSubscriber simply prints the values to console.
        dataSubscriber = new WeatherForecastSubscriber();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{observer} subscribing to {subscription}.", nameof(ExampleStructDataObserver), nameof(DataSubscription<WeatherForecast>));

        dataSubscriber.Subscribe(dataSubscription);

        return Task.CompletedTask;
    }
}