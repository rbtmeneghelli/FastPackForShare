using FastPackForShare.Extensions;

namespace FastPackForShare.Helpers;

public static class HelperSecurity
{
    public static string BuildPassword(int length = 8)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }

    public static void BrokePassword(string password, int sizePassword, string keys)
    {
        // Exemplo: (123Abc, 8, "")

        char firstChar = 'a';
        char lastChar = 'z';
        long retries = 0;
        bool finish = false;

        if (keys == password)
        {
            finish = true;
        }

        if (keys.Length == sizePassword || finish == true)
        {
            return;
        }

        for (char c = firstChar; c <= lastChar; c++)
        {
            retries++;
            BrokePassword(password, sizePassword, keys + c);
        }
    }

    public static bool CheckPassword(string password)
    {
        if (password.Length < 6 || password.Length > 15)
            return false;
        else if (password.Contains(" "))
            return false;
        else if (!password.Any(char.IsUpper))
            return false;
        else if (!password.Any(char.IsLower))
            return false;

        for (int i = 0; i < password.Length - 1; i++)
        {
            if (password[i] == password[i + 1])
                return false;
        }

        string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
        char[] specialCharactersArray = specialCharacters.ToCharArray();
        foreach (char c in specialCharactersArray)
        {
            if (password.Contains(c))
                return true;
        }

        return false;
    }

    public static string CreateStrongPassword(int numCharacters = 10, bool includeSpecialChars = true, bool onlyUpperCase = false)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        const string special = "!@#$%ˆ&*(){}[];";
        var chars = includeSpecialChars ? valid + special : valid;
        var res = new StringBuilder();
        while (0 < numCharacters--)
            res.Append(chars[GetRandomNumber(chars.Length)]);
        return onlyUpperCase ? res.ToString().ApplyTrim() : res.ToString().ApplyTrim();
    }

    public static bool CheckPasswordIsStrong(string password)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(password))
            return false;

        if (GuardClauseExtension.IntervalMaxLengthIsOk(password, 6, 12))
            return false;

        if (GuardClauseExtension.HasAnyDigit(password) == false)
            return false;

        if (GuardClauseExtension.HasAnyUpperChar(password) == false)
            return false;

        if (GuardClauseExtension.HasAnyLowerChar(password) == false)
            return false;

        if (GuardClauseExtension.HasAnySymbolChar(password) == false)
            return false;

        var repeatCount = 0;
        var lastCharacter = '\0';

        foreach (var c in password)
        {
            if (c == lastCharacter)
                repeatCount++;
            else
                repeatCount = 0;

            if (repeatCount == 2)
                return false;

            lastCharacter = c;
        }

        return true;
    }

    private static int GetRandomNumber(int value, int? minValue = null)
    {
        if (minValue.HasValue)
            return Random.Shared.Next(minValue.GetValueOrDefault(0), value);

        return Random.Shared.Next(value);
    }

    public static string GenerateTokenNumbers()
    {
        int count = 0;
        string token = string.Empty;

        while (count < 8)
        {
            int number = GetRandomNumber(9, 0);
            token = string.Concat(token, number.ToString());
        }

        return token.ApplyTrim();
    }

    public static string GenerateRandomPassword()
    {
        return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
            .Select(s => s[GetRandomNumber(s.Length)]).ToArray());
    }
}
