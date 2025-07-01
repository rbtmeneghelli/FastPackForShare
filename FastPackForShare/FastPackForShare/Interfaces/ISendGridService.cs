using FastPackForShare.Models;

namespace FastPackForShare.Interfaces;

public interface ISendGridService
{
    Task SendCustomEmailAsync(SendGridModel sendGridModel);
    Task SendEmailTemplateAsync(SendGridModel sendGridModel);
}
