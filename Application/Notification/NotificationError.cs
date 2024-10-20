namespace Application.Notification;

public class NotificationError : INotificationError
{
    readonly List<NotificationErrorMessage> _notificationErrorMessages;

    public NotificationError()
    {
        _notificationErrorMessages = [];
    }

    public List<NotificationErrorMessage> GetNotifications()
        => _notificationErrorMessages;

    public void Handle(NotificationErrorMessage notification)
        => _notificationErrorMessages.Add(notification);

    public bool HasNotifications()
        => _notificationErrorMessages.Count != 0;
}
