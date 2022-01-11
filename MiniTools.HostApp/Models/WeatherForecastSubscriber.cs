﻿namespace MiniTools.HostApp.Models;


/// <summary>
/// Dumb subscriber that prints received values; better example should do some work value.
/// </summary>
public class WeatherForecastSubscriber : IStructDataSubscriber<WeatherForecast>
{
    private IDisposable? unsubscriber;

    public virtual void Subscribe(IObservable<WeatherForecast> provider)
    {
        unsubscriber = provider.Subscribe(this);
    }

    public virtual void Unsubscribe()
    {
        unsubscriber?.Dispose();
    }

    public void OnCompleted()
    {
        Console.WriteLine("End of data; No more data will not be transmitted.");
    }

    public void OnError(Exception error)
    {
        Console.Error.WriteLine(error.Message);
    }

    public void OnNext(WeatherForecast value)
    {
        Console.WriteLine("{0} is {1} ({2} Celsius)", value.Date, value.Summary, value.TemperatureC);
    }
}
