using Microsoft.AspNetCore.SignalR;

namespace ApartmentManager.API.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", user, message);
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User != null && Context.User.IsInRole("Admin"))
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        await base.OnConnectedAsync();
    }
}