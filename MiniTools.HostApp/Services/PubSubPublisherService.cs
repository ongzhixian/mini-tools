using Azure.Messaging.WebPubSub;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniTools.HostApp.Models;

namespace MiniTools.HostApp.Services;

public class PubSubPublisherService : BackgroundService
{
    private readonly ILogger<PubSubPublisherService> logger;
    private readonly IOptions<AzureWebPubSubSetting> options;
    private WebPubSubServiceClient serviceClient;

    public PubSubPublisherService(ILogger<PubSubPublisherService> logger, IOptions<AzureWebPubSubSetting> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));

        Initialize();

        if (serviceClient == null)
            throw new InvalidOperationException();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(PubSubPublisherService)} is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);

            string message = $"{DateTime.Now} {nameof(PubSubPublisherService)} message";

            await serviceClient.SendToAllAsync(message);

            logger.LogInformation("SENT: [{message}]", message);
        }
    }

    private void Initialize()
    {
        serviceClient = new WebPubSubServiceClient(options.Value.ConnectionString, "myHub1");
    }
}