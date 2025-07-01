using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using FastPackForShare.Constants;
using FastPackForShare.Interfaces;
using FastPackForShare.Models;

namespace FastPackForShare.Services;

public sealed class SendGridService : ISendGridService
{
    // Aqui é trabalhado com Bind de configuração, por isso do IOptions.
    // Pode se passar diretamente a var de ambiente com as config
    private readonly SendGridConfigModel _sendGridModel;
    private readonly SendGridClient _client;
    private readonly EmailAddress _from;

    public SendGridService(IOptions<SendGridConfigModel> sendGridModel)
    {
        _sendGridModel = sendGridModel?.Value ?? throw new ArgumentException("sendGridModel não pode ser nulo", nameof(sendGridModel));
        _client = new SendGridClient(_sendGridModel.ApiKey);
        _from = new EmailAddress(_sendGridModel.EmailSender, _sendGridModel.EmailSenderName);
    }

    public async Task SendCustomEmailAsync(SendGridModel sendGridModel)
    {
        var to = new EmailAddress(sendGridModel.EmailFor);
        var msg = MailHelper.CreateSingleEmail(_from, to, sendGridModel.Subject, sendGridModel.Message, sendGridModel.Message);
        await SendEmailAsync(msg);
    }

    public async Task SendEmailTemplateAsync(SendGridModel sendGridModel)
    {
        var to = new EmailAddress(sendGridModel.EmailFor);
        var msg = new SendGridMessage { From = _from, TemplateId = sendGridModel.TemplateId };
        msg.AddTo(to);
        msg.SetTemplateData(sendGridModel.Data);
        await SendEmailAsync(msg);
    }

    private async Task SendEmailAsync(SendGridMessage sendGridMessage)
    {
        var response = await _client.SendEmailAsync(sendGridMessage);

        if ((int)response.StatusCode >= ConstantHttpStatusCode.BAD_REQUEST_CODE)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new Exception($"Erro ao enviar e-mail: {body}");
        }
    }
}
