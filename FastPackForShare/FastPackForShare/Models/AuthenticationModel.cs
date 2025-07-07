using System.Security.Claims;

namespace FastPackForShare.Models;

public sealed record AuthenticationModel
{
    public long? Id { get; set; }
    public string Login { get; set; }
    public string Initials { get; set; }
    public string Profile { get; set; }
    public ICollection<Claim> Roles { get; set; }
    public string Token { get; set; }
    public DateTime AccessDate { get; set; }
    public string ExpirationDate { get; set; }
    public string CodeTwoFactoryCode { get; set; }
}
