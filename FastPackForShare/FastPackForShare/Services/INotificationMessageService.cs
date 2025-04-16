using FastPackForShare.Models;

namespace FastPackForShare.Services;

public interface INotificationMessageService
{
    void Handle(NotificationMessageModel notificationMessageModel);
    List<NotificationMessageModel> GetNotifications();
    bool HaveNotification();
}
