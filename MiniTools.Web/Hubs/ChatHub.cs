using Microsoft.AspNetCore.SignalR;

namespace MiniTools.Web.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}

public class ChatHub : Hub<IChatClient> // Hub
{
    //public async Task SendMessage(string user, string message)
    //{
    //    await Clients.All.SendAsync("ReceiveMessage", user, message);
    //}

    public async Task SendMessage(string user, string message)
    {

        //var transport = Context.QueryString.First(p => p.Key == "transport").Value;
        var transportType = Context.Features.Get<Microsoft.AspNetCore.Http.Connections.Features.IHttpTransportFeature>()?.TransportType;

        await Clients.All.ReceiveMessage(user, message);
    }

    public Task SendMessageToCaller(string user, string message)
    {
        return Clients.Caller.ReceiveMessage(user, message);
    }
}
