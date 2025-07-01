namespace FastPackForShare.Models;

public record SendGridConfigModel
{
    public string ApiKey { get; set; }
    public string Client { get; set; }
    public string EmailSender { get; set; }
    public string EmailSenderName { get; set; }
}
