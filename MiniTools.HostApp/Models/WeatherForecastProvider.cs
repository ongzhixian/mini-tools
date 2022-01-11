using System.Text.Json;

namespace MiniTools.HostApp.Models
{
    public class WeatherForecastProvider : IStructDataPublisher<WeatherForecast>
    {
        private readonly DataSubscription<WeatherForecast> subscription;

        public WeatherForecastProvider(DataSubscription<WeatherForecast> subscription)
        {
            this.subscription = subscription;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task PublishDataAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stopToken);

                var mockWeatherForecastList = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray();

                foreach (var weatherForecast in mockWeatherForecastList)
                {
                    Thread.Sleep(2500);

                    Console.WriteLine(weatherForecast.Summary);

                    foreach (var observer in subscription.Observers)
                        observer.OnNext(weatherForecast);
                }
            }

            foreach (var observer in subscription.Observers.ToArray())
                observer?.OnCompleted();
            subscription.Observers.Clear();
        }
    }

}
