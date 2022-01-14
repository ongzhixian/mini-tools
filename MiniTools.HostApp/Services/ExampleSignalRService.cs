using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniTools.HostApp.Services;

public class ExampleSignalRService : BackgroundService
{
    private readonly ILogger<ExampleSignalRService> logger;
    HubConnection connection;
    HubConnection chatHubConnection;
    string? jwt = string.Empty;
    bool completedInitialization;

    public ExampleSignalRService(ILogger<ExampleSignalRService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        EventBus<Jwt>.Instance.NewData += Instance_NewData;

        //var subscriber = new MessageBusSubscriber<Jwt>(MessageBus<Jwt>.Instance);
        //subscriber.NewValueAction = (jwt) =>
        //{
        //    Console.WriteLine("Message bus received " + jwt.Token);
        //};
        //MessageBus<Jwt>.Instance.Subscribe(subscriber);


        //var subscription = MessageBus22Draft.Instance.Subscribe<Jwt>();
        //subscription.OnNewData = (jwt) =>
        //{
        //    Console.WriteLine("Bus22 received " + jwt.Token);
        //};
        //subscription.OnNewData = OnNew;


        jwt = "eyjhbgcioijiuzi1niisinr5cci6ikpxvcj9.eyjodhrwoi8vc2nozw1hcy54bwxzb2fwlm9yzy93cy8ymda1lza1l2lkzw50axr5l2nsywltcy9uyw1lijoizgv2iiwiahr0cdovl3njagvtyxmubwljcm9zb2z0lmnvbs93cy8ymda4lza2l2lkzw50axr5l2nsywltcy9yb2xlijpbikfkbwluaxn0cmf0b3iilcjezxzlbg9wzxiilcjnevbyb2zpbguixswizxhwijoxnjqymtq4mjg3lcjpc3mioijodhrwczovl2xvy2fsag9zddo3mdaxlyisimf1zci6imh0dhbzoi8vbg9jywxob3n0ojcwmdevin0.5cet5sdy5-kexnlmposiugteljuxi0rogqhnxxlo7i0";
        initializehubconnection();
    }

    private void OnNew(Jwt jwt)
    {
        Console.WriteLine("Bus22 received " + jwt.Token);
    }

    private void Instance_NewData(object? sender, Jwt e)
    {
        Console.WriteLine("SignalR received " + e.Token);

        jwt = e.Token;

        if ((!string.IsNullOrEmpty(jwt)) && (!completedInitialization))
        {
            InitializeHubConnection();
        }
    }

    private void InitializeHubConnection()
    {
        InitializeChatHubConnection();

        InitializeClockHubConnection();

        //await connection.StartAsync(stoppingToken);

        //await chatHubConnection.StartAsync(stoppingToken);

        completedInitialization = true;
    }

    private void InitializeClockHubConnection()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7001/hubs/clock", options =>
            {
                // Authentication via Bearer
                // options.AccessTokenProvider = () => Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGV2IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbIkFkbWluaXN0cmF0b3IiLCJEZXZlbG9wZXIiLCJNeVByb2ZpbGUiXSwiZXhwIjoxNjQyMDUwNTg4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDAxLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMDEvIn0.R7EXROt638gOmLVfzxCiIvVUpHDGCvzC9p2IDlq4vPs")
                options.AccessTokenProvider = () => Task.FromResult(jwt);
            })
            .Build();


        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };

        connection.On<string>("ShowTime", (message) =>
        {

            logger.LogInformation("Server clock: {message}", message);
        });

        // Example

    }

    private void InitializeChatHubConnection()
    {
        chatHubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7001/chatHub")
                .WithAutomaticReconnect()
                .Build();

        chatHubConnection.Closed += ChatHubConnection_Closed;
        chatHubConnection.Reconnected += ChatHubConnection_Reconnected;
        chatHubConnection.Reconnecting += ChatHubConnection_Reconnecting;

        //chatHubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        //{
        //    //this.Dispatcher.Invoke(() =>
        //    //{
        //    //    var newMessage = $"{user}: {message}";
        //    //    messagesList.Items.Add(newMessage);
        //    //});
        //    logger.LogInformation(message);
        //});
    }

    private Task ChatHubConnection_Reconnecting(Exception? arg)
    {
        Console.WriteLine("ChatHubConnection_Reconnecting");
        return Task.CompletedTask;
    }

    private Task ChatHubConnection_Reconnected(string? arg)
    {
        Console.WriteLine("ChatHubConnection_Reconnected");
        return Task.CompletedTask;
    }

    private Task ChatHubConnection_Closed(Exception? arg)
    {
        Console.WriteLine("ChatHubConnection_Closed");
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!completedInitialization)
        {
            await Task.Delay(2500, stoppingToken);
        }

        await connection.StartAsync(stoppingToken);

        await chatHubConnection.StartAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {

                logger.LogInformation("chatHubConnection state is {state}", HubConnectionState.Connected);

                // we need to if (chatHubConnection.State == HubConnectionState.Connected)

                await chatHubConnection.InvokeAsync("SendMessage", "some-user", "some message", cancellationToken: stoppingToken);

            }
            catch (Exception ex)
            {
                throw;
            }

            await Task.Delay(2500, stoppingToken);
        }
    }

}