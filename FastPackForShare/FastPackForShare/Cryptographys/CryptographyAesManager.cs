namespace FastPackForShare.Cryptography;

/// <summary>
/// CRIPTOGRAFIA MAIS INDICADA A SER UTILIZADA PARA FUTUROS PROJETOS...
/// </summary>
public static class CryptographyAesManager
{
    public static string ApplyDecryptd(byte[] cryptText)
    {
        string decryptText = string.Empty;

        using (Aes aesAlg = Aes.Create())
        {
            byte[] chave = aesAlg.Key;
            byte[] iv = aesAlg.IV;
            decryptText = DecryptStringAES(cryptText, chave, iv);
        }

        return decryptText;
    }

    public static string ApplyHashCodeAndEncrypt(string text)
    {
        string hashCode = CalculateHashSha3_256(text);
        return ApplyEncrypt(hashCode);
    }

    public static bool ComparePassword(string passwordUser, string passwordUserDigit)
    {
        return passwordUser.Equals(ApplyHashCodeAndEncrypt(passwordUserDigit));
    }

    private static byte[] EncryptStringAES(string plainText, byte[] key, byte[] IV)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }
    }

    private static string DecryptStringAES(byte[] cipherText, byte[] key, byte[] IV)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    private static string CalculateHashSha3_256(string texto)
    {
        using (SHA3_256 sha256 = SHA3_256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(texto);
            byte[] hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    private static string ApplyEncrypt(string text)
    {
        string cryptText = string.Empty;

        using (Aes aesAlg = Aes.Create())
        {
            byte[] chave = aesAlg.Key;
            byte[] iv = aesAlg.IV;
            byte[] arrCryptText = EncryptStringAES(text, chave, iv);
            cryptText = Convert.ToBase64String(arrCryptText);
        }

        return cryptText;
    }
}