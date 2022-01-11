using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniTools.HostApp.Models;

namespace MiniTools.HostApp.Services;

public class ExampleAirTemperatureObserver : BackgroundService
{
    private readonly ILogger<ExampleAirTemperatureObserver> logger;
    private readonly DataSubscription<AirTemperatureInfo> dataSubscription;
    private readonly IDataSubscriber<AirTemperatureInfo> dataSubscriber;

    public ExampleAirTemperatureObserver(ILogger<ExampleAirTemperatureObserver> logger,
        DataSubscription<AirTemperatureInfo> dataSubscription)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.dataSubscription = dataSubscription;

        // Subscriber should be a class that acts on the data received through the subscription
        // In this example, WeatherForecastSubscriber simply prints the values to console.
        dataSubscriber = new AirTemperatureInfoSubscriber();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{observer} subscribing to {subscription}.", nameof(ExampleAirTemperatureObserver), nameof(DataSubscription<WeatherForecast>));

        dataSubscriber.Subscribe(dataSubscription);

        return Task.CompletedTask;
    }
}