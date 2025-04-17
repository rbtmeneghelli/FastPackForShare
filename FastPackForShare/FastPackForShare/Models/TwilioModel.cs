namespace FastPackForShare.Models;

public sealed record TwilioModel
{
    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string TwilioNumber { get; set; }
}
