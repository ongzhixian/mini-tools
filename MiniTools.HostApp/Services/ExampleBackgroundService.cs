using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniTools.HostApp.Services;

public class ExampleBackgroundService : BackgroundService
{
    private readonly ILogger<ExampleBackgroundService> logger;
    readonly IChannelQueueService queueService;

    public ExampleBackgroundService(ILogger<ExampleBackgroundService> logger, IChannelQueueService service)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        queueService = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ExampleBackgroundService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation($"ExampleBackgroundService task doing background work.");

            // Do some work here

            string fullPath = System.IO.Path.GetFullPath("/data");

            logger.LogInformation("{dataPath}", fullPath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var res = Environment.GetEnvironmentVariable("backendUrl");
            logger.LogInformation("res is {res}", res);

            var rts = Environment.GetEnvironmentVariable("RUNTIME_SERVICE");
            logger.LogInformation("rts is {rts}", rts);

            await queueService.EnqueueAsync(stopToken =>
            {
                return new ValueTask(Task.Run(() =>
                {
                    Console.WriteLine("DO SOMETING");
                }, stoppingToken));
            });


            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("/data/DUMMY.log"))
            {
                sw.AutoFlush = true;
                sw.WriteLine("it s a small world");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}