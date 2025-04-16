namespace FastPackForShare.Models;

public sealed class RefreshTokensModel
{
    public string Username { get; set; }
    public string RefreshToken { get; set; }

    public RefreshTokensModel(string userName, string refreshToken)
    {
        Username = userName;
        RefreshToken = refreshToken;
    }
}
