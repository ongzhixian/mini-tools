using Azure.Messaging.WebPubSub;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniTools.HostApp.Models;
using System.Net.WebSockets;
using System.Text;

namespace MiniTools.HostApp.Services;

public class PubSubConsumerService : BackgroundService
{
    private readonly ILogger<PubSubConsumerService> logger;
    private readonly IOptions<AzureWebPubSubSetting> options;
    private WebPubSubServiceClient serviceClient;
    private readonly Memory<byte> buffer = new Memory<byte>(new byte[256 * 1024]); // Assumes max message is 256 kb

    public PubSubConsumerService(ILogger<PubSubConsumerService> logger, IOptions<AzureWebPubSubSetting> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        
        Initialize();

        if (serviceClient == null)
            throw new InvalidOperationException();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(PubSubConsumerService)} is starting.");
        
        using (var client = new ClientWebSocket())
        {
            logger.LogInformation("Connecting...");

            await client.ConnectAsync(serviceClient.GetClientAccessUri(cancellationToken: stoppingToken), stoppingToken);

            logger.LogInformation("Connected.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var recvResult = await client.ReceiveAsync(buffer, stoppingToken);

                if (recvResult.MessageType != WebSocketMessageType.Text)
                {
                    logger.LogWarning("RECV: [{message}]", recvResult.MessageType);
                    continue;
                }

                string recvMessage = Encoding.UTF8.GetString(buffer[..recvResult.Count].ToArray());
                
                logger.LogInformation("RECV: [{message}]", recvMessage);
            }
        }
    }

    private void Initialize()
    {
        serviceClient = new WebPubSubServiceClient(options.Value.ConnectionString, "myHub1");
    }
}