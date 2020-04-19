namespace MyStock.WebAPI.Notifications.Abstraction
{
    public interface INotification
    {
        NotificationType NotificationType { get; set; }
    }
}