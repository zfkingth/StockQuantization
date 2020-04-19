
using MyStock.WebAPI.Notifications.Abstraction;

namespace MyStock.WebAPI.Notifications.Models
{
  public class Notification<T> : INotification
  {
    public NotificationType NotificationType { get; set; }
    public T Payload { get; set; }

  }
}