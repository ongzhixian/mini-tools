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

            string fullPath = System.IO.Path.GetFullPath("/data");

            logger.LogInformation(fullPath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var res = Environment.GetEnvironmentVariable("backendUrl");
            logger.LogInformation("res is {res}", res);

            var rts = Environment.GetEnvironmentVariable("RUNTIME_SERVICE");
            logger.LogInformation("rts is {rts}", rts);

            


            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("/data/DUMMY.log"))
            {
                sw.AutoFlush = true;
                sw.WriteLine("it s a small world");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}