using System.Text.Json;

namespace MiniTools.HostApp.Models
{
    public class AirTemperatureInfoProvider : IDataPublisher<AirTemperatureInfo>
    {
        private readonly DataSubscription<AirTemperatureInfo> subscription;
        private readonly HttpClient http;

        public AirTemperatureInfoProvider(DataSubscription<AirTemperatureInfo> subscription)
        {
            this.subscription = subscription;
            http = new HttpClient();
        }

        public async Task PublishDataAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                await Task.Delay(2500, stopToken);

                // Original url used was https://api.data.gov.sg/v1/environment/air-temperature?date=yyyy-MM-dd
                // Using just `date` gets all temperature for the specified day; lets get lesser data using date_time
                string url = $"https://api.data.gov.sg/v1/environment/air-temperature?date_time={DateTime.Now:yyyy-MM-ddTHH:mm:ss}";

                using HttpResponseMessage? responseMessage = await http.GetAsync(url, stopToken);

                try
                {
                    responseMessage.EnsureSuccessStatusCode();

                    var json = await responseMessage.Content.ReadAsStringAsync(stopToken);

                    var info = JsonSerializer.Deserialize<AirTemperatureInfo>(json);

                    if (info != null)
                        foreach (var observer in subscription.Observers)
                            observer.OnNext(info);
                }
                catch (Exception)
                {
                    break;
                }
            }

            var allObservers = subscription.Observers.ToArray();
            subscription.Observers.Clear();
            foreach (var observer in allObservers)
                observer?.OnCompleted();
        }

    }

}
