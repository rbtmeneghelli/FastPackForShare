using System.Security.Claims;
using FastPackForShare.Models;

namespace FastPackForShare.Services;

public interface ITokenService
{
    string GenerateToken(AuthenticationModel authenticationModel);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    void SaveRefreshToken(string username, string refreshToken);
    string GetRefreshToken(string username);
    void DeleteRefreshToken(string username, string refreshToken);
    bool ValidateTokenAuthentication(string token);
}
