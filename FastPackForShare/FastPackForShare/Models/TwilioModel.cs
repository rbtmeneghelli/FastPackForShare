namespace FastPackForShare.Models;

public sealed record TwilioModel
{
    public required string AccountSid { get; init; }
    public required string AuthToken { get; init; }
    public required string TwilioNumber { get; init; }

    public TwilioModel(string accountSid, string authToken, string twilioNumber)
    {
        AccountSid = accountSid;
        AuthToken = authToken;
        TwilioNumber = twilioNumber;
    }
}
