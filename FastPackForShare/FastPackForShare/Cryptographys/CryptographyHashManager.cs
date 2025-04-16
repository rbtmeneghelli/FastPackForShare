namespace FastPackForShare.Cryptography;

public class CryptographyHashManager
{
    private HashAlgorithm _algorithm;

    public CryptographyHashManager(HashAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    public string EncryptPassword(string password)
    {
        var encodedValue = Encoding.UTF8.GetBytes(password);
        var encryptedPassword = _algorithm.ComputeHash(encodedValue);

        var sb = new StringBuilder();
        foreach (var caracter in encryptedPassword)
            sb.Append(caracter.ToString("X2"));

        return sb.ToString();
    }

    public bool CheckPassword(string passwordDigitByUser, string passwordSavedByUser)
    {
        var encryptedPassword = _algorithm.ComputeHash(Encoding.UTF8.GetBytes(passwordDigitByUser));

        var sb = new StringBuilder();
        foreach (var caractere in encryptedPassword)
        {
            sb.Append(caractere.ToString("X2"));
        }

        return sb.ToString() == passwordSavedByUser;
    }
}
