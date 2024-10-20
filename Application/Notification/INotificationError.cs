namespace Application.Notification;

public interface INotificationError
{
    void Handle(NotificationErrorMessage notification);
    List<NotificationErrorMessage> GetNotifications();
    bool HasNotifications();
}
