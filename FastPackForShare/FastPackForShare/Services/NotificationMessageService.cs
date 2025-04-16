using FastPackForShare.Interfaces;
using FastPackForShare.Models;

namespace FastPackForShare.Services;

public class NotificationMessageService : INotificationMessageService
{
    private List<NotificationMessageModel> _notifications;

    public NotificationMessageService()
    {
        _notifications = new List<NotificationMessageModel>();
    }

    public void Handle(NotificationMessageModel notification)
    {
        _notifications.Add(notification);
    }

    public List<NotificationMessageModel> GetNotifications()
    {
        return _notifications;
    }

    public bool HaveNotification()
    {
        return _notifications?.Count() > 0;
    }
}
