﻿namespace FastPackForShare.Cryptography;

public sealed class CryptographyManager
{
    private const int SaltSize = 16;
    private const int HashSize = 20;

    public string Hash(string password, int iterations)
    {
        //create salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
        //create hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);
        //combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
        //convert to base64
        var base64Hash = Convert.ToBase64String(hashBytes);
        //format hash with extra information
        return $"$MYHASH$V1${iterations}${base64Hash}";
    }

    public string Hash(string password)
    {
        return Hash(password, 10000);
    }

    public bool IsHashSupported(string hashString)
    {
        return hashString.Contains("$MYHASH$V1$");
    }

    public bool Verify(string password, string hashedPassword)
    {
        // check hash
        if (!IsHashSupported(hashedPassword))
            return false;

        //extract iteration and Base64 string
        var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
        var iterations = int.Parse(splittedHashString[0]);
        var base64Hash = splittedHashString[1];

        //get hashbytes
        var hashBytes = Convert.FromBase64String(base64Hash);

        //get salt
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        //create hash with given salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        //get result
        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }
        return true;
    }
}
