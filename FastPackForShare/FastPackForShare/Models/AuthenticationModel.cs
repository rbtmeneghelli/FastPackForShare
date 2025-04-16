using System.Security.Claims;

namespace FastPackForShare.Models;

public sealed record AuthenticationModel
{
    public long? Id { get; set; }
    public string Login { get; set; }
    public string Profile { get; set; }
    public IEnumerable<Claim> Roles { get; set; }
    public string Token { get; set; }
    public string AccessDate { get; set; }
    public string ExpirationDate { get; set; }
}
