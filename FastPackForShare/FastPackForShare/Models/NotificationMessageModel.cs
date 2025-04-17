namespace FastPackForShare.Models;

public sealed class NotificationMessageModel
{
    public string Message { get; }

    public NotificationMessageModel(string message)
    {
        Message = message;
    }
}
