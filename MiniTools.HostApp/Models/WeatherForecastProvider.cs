namespace MiniTools.HostApp.Models
{
    // Observation (data) object
    public struct WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }

    public interface IStructDataPublisher<T> where T : struct
    {
        void PublishData();
    }

    // Provider

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

        public void PublishData()
        {
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

            //
            //foreach (var observer in observers.ToArray())
            //    observer?.OnCompleted();
            //observers.Clear();
        }

    }

}
