namespace FastPackForShare.Cryptographys;

public static class CryptographyBcryptManager
{
    public static string ApplyHashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public static bool ValidateHash(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
