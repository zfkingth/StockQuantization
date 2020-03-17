using Microsoft.AspNetCore.SignalR;

namespace Stock.WebAPI.Notifications
{
  public class UserIdProvider : IUserIdProvider
  {
    public string GetUserId(HubConnectionContext connection)
    {
      return connection.User.Identity.Name;
    }
  }
}