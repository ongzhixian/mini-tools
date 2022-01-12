using Microsoft.AspNetCore.SignalR;
using MiniTools.Web.Hubs;

namespace MiniTools.Web.Services;


public class ClockBackgroundService : BackgroundService
{
    private readonly ILogger<ClockBackgroundService> _logger;
    private readonly IHubContext<ClockHub, IClock> _clockHub;
    private readonly IHubContext<ChatHub, IChatClient> _chatHub;

    public ClockBackgroundService(ILogger<ClockBackgroundService> logger, 
        IHubContext<ClockHub, IClock> clockHub,
        IHubContext<ChatHub, IChatClient> chatHub
        )
    {
        _logger = logger;
        _clockHub = clockHub;
        _chatHub = chatHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTime.Now);

            await _clockHub.Clients.All.ShowTime("fake");


            //await _chatHub.Clients.All.ReceiveMessage("asd", DateTime.Now.ToString());
            await Task.Delay(1000);
        }
    }
}