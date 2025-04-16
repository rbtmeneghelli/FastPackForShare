using FastPackForShare.Models;

namespace FastPackForShare.Interfaces;

public interface INotificationMessageService
{
    void Handle(NotificationMessageModel notificationMessageModel);
    List<NotificationMessageModel> GetNotifications();
    bool HaveNotification();
}
