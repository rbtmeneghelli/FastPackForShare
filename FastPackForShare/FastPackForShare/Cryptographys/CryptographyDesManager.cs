namespace FastPackForShare.Cryptography;

public static class CryptographyDesManager
{
    public static string EncryptTextToMemory(string data)
    {
        MemoryStream mStream = new MemoryStream();

        DES DESalg = DES.Create();

        CryptoStream cStream = new CryptoStream(mStream,
            DESalg.CreateEncryptor(DESalg.Key, DESalg.IV),
            CryptoStreamMode.Write);

        byte[] toEncrypt = new ASCIIEncoding().GetBytes(data);

        cStream.Write(toEncrypt, 0, toEncrypt.Length);
        cStream.FlushFinalBlock();

        byte[] ret = mStream.ToArray();

        cStream.Close();
        mStream.Close();

        return Encoding.ASCII.GetString(ret);
    }

    public static string DecryptTextFromMemory(string data)
    {

        if (GuardClauseExtension.IsNullOrWhiteSpace(data) == false)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            MemoryStream msDecrypt = new MemoryStream(byteData);

            DES DESalg = DES.Create();

            CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                DESalg.CreateDecryptor(DESalg.Key, DESalg.IV),
                CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[data.Length];

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return new ASCIIEncoding().GetString(fromEncrypt);
        }

        return data;
    }
}
