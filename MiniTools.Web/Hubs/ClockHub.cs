using Microsoft.AspNetCore.SignalR;

namespace MiniTools.Web.Hubs;

public interface IClock
{
    Task ShowTime(string currentTime);
}

public class ClockHub : Hub<IClock>
{
    //public async Task SendTimeToClients(DateTime dateTime)
    //{
    //    await Clients.All.ShowTime(
    //        $"Report time to {Context.User} {dateTime}"
    //        );
    //}
}