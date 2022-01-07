using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniTools.HostApp.Models;

namespace MiniTools.HostApp.Services;

public class QueueConsumerService : BackgroundService
{
    private readonly ILogger<QueueConsumerService> logger;
    private readonly IOptions<AzureStorageSetting> options;
    QueueClient? queueClient;

    public QueueConsumerService(ILogger<QueueConsumerService> logger, IOptions<AzureStorageSetting> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(QueueConsumerService)} is starting.");

        await InitializeAsync(stoppingToken);

        if (queueClient == null)
            throw new InvalidOperationException();

        while (!stoppingToken.IsCancellationRequested)
        {
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 10, cancellationToken: stoppingToken);

            foreach (var item in messages)
            {
                logger.LogInformation("Received: [{messageBody}] -- dequeueCount: {dequeueCount} {nextVisibleOn:HH:mm:ss} {expiresOn:yy-MM-dd}",
                    item.Body,
                    item.DequeueCount,
                    item.NextVisibleOn,
                    item.ExpiresOn
                    );

                // Dequeue
                // await queueClient.DeleteMessageAsync(item.MessageId, item.PopReceipt, stoppingToken);

                // Dispatch item
                // return item;
            }

            logger.LogInformation("No more message");

            await Task.Delay(10000, stoppingToken);
        }
    }

    private async Task InitializeAsync(CancellationToken stopToken)
    {
        // TODO: Options?
        // Queue names
        // 1. must consist of lowercase letters / digits / hyphens [a-z0-9]-
        // 2. must start with letter or digit
        // 3. must be between 3 to 63 characters long
        // 4. must not have consecutive hyphens
        const string queueName = "test-queue1";

        queueClient = new QueueClient(options.Value.ConnectionString, queueName);

        var queueExist = await queueClient.ExistsAsync(stopToken);

        if (queueExist)
        {
            logger.LogInformation($"Using existing queue: {queueName}");

            return;
        }

        logger.LogInformation($"Creating queue: {queueName}");

        await queueClient.CreateAsync(cancellationToken: stopToken);
    }
}