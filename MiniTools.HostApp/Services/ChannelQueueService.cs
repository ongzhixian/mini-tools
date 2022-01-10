using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace MiniTools.HostApp.Services;


public interface IChannelQueueService
{
    ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

public class ChannelQueueService : IChannelQueueService // BackgroundService
{
    private readonly ILogger<ChannelQueueService> logger;
    private readonly Channel<Func<CancellationToken, ValueTask>> channel;


    public ChannelQueueService(ILogger<ChannelQueueService> logger, int capacity = int.MaxValue)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        channel = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await channel.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? workItem = await channel.Reader.ReadAsync(cancellationToken);

        return workItem;
    }

}