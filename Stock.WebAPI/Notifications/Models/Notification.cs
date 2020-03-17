
using Stock.WebAPI.Notifications.Abstraction;

namespace Stock.WebAPI.Notifications.Models
{
  public class Notification<T> : INotification
  {
    public NotificationType NotificationType { get; set; }
    public T Payload { get; set; }

  }
}