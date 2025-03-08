
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


public class NotificationHub : Hub
{
    public async Task SendOrderNotification(string message, int banId)
    {
        await Clients.All.SendAsync("ReceiveOrderNotification", new
        {
            message = message,
            banId = banId
        });
    }

}