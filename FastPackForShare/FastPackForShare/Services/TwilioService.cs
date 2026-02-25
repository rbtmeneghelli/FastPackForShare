using FastPackForShare.Interfaces;
using FastPackForShare.Models;
using FastPackForShare.Services.Bases;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FastPackForShare.Services;

public sealed class TwilioService : BaseHandlerService, ITwilioService
{
    private TwilioModel _TwilioModel { get; set; }

    public TwilioService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    public void SetTwilioSettingsByEnvironmentVariables(TwilioModel twilioModel) => _TwilioModel = twilioModel;

    public async Task SendWhatsAppMessageAsync(string numberTo, string bodyMessage)
    {
        await SendMessageAsync(numberTo, bodyMessage, true);
    }

    public async Task SendSmsMessageAsync(string numberTo, string bodyMessage)
    {
        await SendMessageAsync(numberTo, bodyMessage, false);
    }

    private async Task SendMessageAsync(string numberTo, string bodyMessage, bool isWhatsApp)
    {
        TwilioClient.Init(_TwilioModel.AccountSid, _TwilioModel.AuthToken);

        if (isWhatsApp)
        {
            await MessageResource.CreateAsync(
                body: bodyMessage,
                from: new PhoneNumber($@"whatsapp:{_TwilioModel.TwilioNumber}"),
                to: new PhoneNumber($@"whatsapp:{numberTo}")
            );
        }
        else
        {
            await MessageResource.CreateAsync(
                body: bodyMessage,
                from: new PhoneNumber(_TwilioModel.TwilioNumber),
                to: new PhoneNumber(numberTo)
            );
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}