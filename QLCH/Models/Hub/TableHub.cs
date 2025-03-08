using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class TableHub : Hub
{
    public async Task UpdateTableStatus(int banId, bool isInUse)
    {
        await Clients.All.SendAsync("ReceiveTableUpdate", banId, isInUse);
    }
}
