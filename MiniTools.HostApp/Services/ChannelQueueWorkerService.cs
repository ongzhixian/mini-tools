using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniTools.HostApp.Services;

public class ChannelQueueWorkerService : BackgroundService
{
    private readonly ILogger<ChannelQueueWorkerService> logger;
    private readonly IChannelQueueService queueService;

    public ChannelQueueWorkerService(
        ILogger<ChannelQueueWorkerService> logger,
         IChannelQueueService queueService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(ChannelQueueWorkerService)} is starting.");

        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask>? workItem = await queueService.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"{nameof(ChannelQueueWorkerService)} is stopping.");

        await base.StopAsync(cancellationToken);
    }
}