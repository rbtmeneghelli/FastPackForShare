using FastPackForShare.Models;

namespace FastPackForShare.Interfaces;

public interface ITwilioService : IDisposable
{
    void SetTwilioSettingsByEnvironmentVariables(TwilioModel twilioModel);
    Task SendSmsMessageAsync(string numberTo, string bodyMessage);
    Task SendWhatsAppMessageAsync(string numberTo, string bodyMessage);
}
