using FastPackForShare.Enums;

namespace FastPackForShare.Models;

/// <summary>
/// A partir do C#15 e NET 11 temos a palavra chave closed, onde ele impede a derivação de outras classes
/// A palavra chave closed e utilizada antes da declaração de record ou class
/// Exemplo: public closed record BaseEmailModel ou public closed class record BaseEmailModel
/// </summary>
public record BaseEmailModel
{
    public string EmailFrom { get; set; }
    public string EmailTo { get; set; }
    public string EmailHost { get; init; }
    public string EmailUser { get; init; }
    public string EmailPassword { get; init; }
    public int EmailPort { get; init; }
    public bool EmailSSL { get; init; }
};

public sealed record GmailModel : BaseEmailModel
{
    public GmailModel(string emailFrom, string emailTo)
    {
        EmailFrom = emailFrom;
        EmailTo = emailTo;
        EmailHost = "smtp.hotmail.com";
        EmailUser = "XPTO@hotmail.com";
        EmailPassword = "PSW@12345#";
        EmailPort = 25;
        EmailSSL = true;
    }
}

public sealed record YahooModel : BaseEmailModel
{
    public YahooModel(string emailFrom, string emailTo)
    {
        EmailFrom = emailFrom;
        EmailTo = emailTo;
        EmailHost = "smtp.hotmail.com";
        EmailUser = "XPTO@hotmail.com";
        EmailPassword = "PSW@12345#";
        EmailPort = 25;
        EmailSSL = true;
    }
}

public sealed record HotmailModel : BaseEmailModel
{
    public HotmailModel(string emailFrom, string emailTo)
    {
        EmailFrom = emailFrom;
        EmailTo = emailTo;
        EmailHost = "smtp.hotmail.com";
        EmailUser = "XPTO@hotmail.com";
        EmailPassword = "PSW@12345#";
        EmailPort = 25;
        EmailSSL = true;
    }
}

public static class EmailFactoryModel
{
    public static BaseEmailModel CreateEmailConfiguration(string emailFrom, string emailTo, EnumSmtpEmail enumSmtpEmail)
    {
        return enumSmtpEmail switch 
        {
            EnumSmtpEmail.Gmail => new GmailModel(emailFrom, emailTo),
            EnumSmtpEmail.Yahoo => new YahooModel(emailFrom, emailTo),
            EnumSmtpEmail.Hotmail => new HotmailModel(emailFrom, emailTo),
            _ => throw new NotImplementedException() // Com a palavra closed, não precisamos desse trecho de código mais
        };
    }
}