using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyStock.WebAPI.Notifications
{
    [Authorize]
    public class NotificationsHub : Hub {

        //public override async Task OnConnectedAsync()
        //{
        //    await base.OnConnectedAsync();
        //}
    }
}