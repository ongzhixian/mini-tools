using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniTools.HostApp.Services;

public class ExampleSignalRService : BackgroundService
{
    private readonly ILogger<ExampleSignalRService> logger;
    HubConnection connection;
    HubConnection chatHubConnection;

    public ExampleSignalRService(ILogger<ExampleSignalRService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // No Auth :-(
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio#connect-to-a-hub

        connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7001/hubs/clock", options =>
                {
                    // Authentication via Bearer
                    options.AccessTokenProvider = () => Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGV2IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbIkFkbWluaXN0cmF0b3IiLCJEZXZlbG9wZXIiLCJNeVByb2ZpbGUiXSwiZXhwIjoxNjQyMDUwNTg4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDAxLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMDEvIn0.R7EXROt638gOmLVfzxCiIvVUpHDGCvzC9p2IDlq4vPs");
                })
                .Build();

        chatHubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7001/chatHub")
                .Build();


        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };

        connection.On<string>("ShowTime", (message) =>
        {
            //this.Dispatcher.Invoke(() =>
            //{
            //    var newMessage = $"{user}: {message}";
            //    messagesList.Items.Add(newMessage);
            //});
            logger.LogInformation(message);
        });


        //connection.On<string, string>("ReceiveMessage", (user, message) =>
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        var newMessage = $"{user}: {message}";
        //        messagesList.Items.Add(newMessage);
        //    });
        //});

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await connection.StartAsync(stoppingToken);

        await chatHubConnection.StartAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await chatHubConnection.InvokeAsync("SendMessage", "some-user", "some message", cancellationToken: stoppingToken);

            await Task.Delay(2500, stoppingToken);
        }
        

    }
}