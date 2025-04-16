namespace FastPackForShare.Models;

public sealed record JwtConfigModel
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expired { get; set; }
}
