using System.Security.Claims;
using FastPackForShare.Models;

namespace FastPackForShare.Interfaces;

public interface ITokenService
{
    string GenerateToken(AuthenticationModel authenticationModel);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    void SaveRefreshToken(string username, string refreshToken);
    string GetRefreshToken(string username);
    void DeleteRefreshToken(string username, string refreshToken);

    #region Metodos para gerar token de redefinição de senha até 24horas, por exemplo

    bool ValidateTokenAuthentication(string token);
    string GenerateTokenRescuePassword(string email, string jwtSecretKey);

    #endregion
}
