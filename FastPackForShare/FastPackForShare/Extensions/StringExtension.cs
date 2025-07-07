using FastPackForShare.Enums;

namespace FastPackForShare.Extensions;

public static class StringExtension
{
    public static string ApplyTrim(this string text) => text.Trim();
    public static string ApplyReplace(this string text, string oldChar, string newChar) => GuardClauseExtension.IsNullOrWhiteSpace(text) == false ? text.Replace(oldChar, newChar) : text;
    public static string SerializeObject(this object data) => JsonSerializer.Serialize(data);
    public static TSource DeserializeObject<TSource>(this string data) => JsonSerializer.Deserialize<TSource>(data);
    public static StringBuilder BuildString(List<string> listStrings, bool hasWordBreak)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var itemString in listStrings)
        {
            if (hasWordBreak)
                sb.AppendLine(itemString);
            else
                sb.Append(itemString).Append('\n');
        }

        return sb;
    }
    public static int GetFirstIndexPositionFromWord(string value, string valueToFind, int indexStart = 0)
    {
        if (value.IndexOf(valueToFind) != -1 && indexStart == 0)
            return value.IndexOf(valueToFind);

        else if (value.IndexOf(valueToFind) != -1 && indexStart > 0)
            return value.IndexOf(valueToFind, indexStart);

        return -1;
    }
    public static int GetLastIndexPositionFromWord(string value, string valueToFind, int indexStart = 0)
    {
        if (value.LastIndexOf(valueToFind) != -1 && indexStart == 0)
            return value.LastIndexOf(valueToFind);

        else if (value.LastIndexOf(valueToFind) != -1 && indexStart > 0)
            return value.LastIndexOf(valueToFind, indexStart);

        return -1;
    }
    public static string TransformListOrArrayInString(IEnumerable<string> list)
    {
        if (list is not null)
            return string.Join(",", list);

        return string.Empty;
    }
    public static string TurnFirstWordFromLetterToUpperCase(this string text, string language = "pt-BR")
    {
        var textResult = CultureInfo.GetCultureInfo(language).TextInfo;
        return textResult.ToTitleCase(text);
    }
    public static string ApplyRemoveAccent(this string value)
    {
        return new string(value
        .Normalize(NormalizationForm.FormD)
        .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
        .ToArray());
    }
    public static string RemoveSpaceFromWords(this string text)
    {
        if (!string.IsNullOrEmpty(text))
            return Regex.Replace(text.ToUpper().Trim(), @"\s+", "");

        return string.Empty;
    }
    public static string FormatCpfOrCnpj(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length == 11)
            return text.Insert(3, ".").Insert(7, ".").Insert(11, "-");
        else if (text.Length == 14)
            return text.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
        else
            return text;
    }
    public static string RemoveFormatCpfCnpj(this string text)
    {
        string[] arrSpecialChar = [".", "-", "/"];
        for (int i = 0; i < arrSpecialChar.Length - 1; i++)
        {
            text = text.ApplyReplace(arrSpecialChar[i], string.Empty);
        }
        return text;
    }
    public static string TransformBoolToString(this bool varBoolean) => varBoolean ? "1" : "0";
    public static bool TransformStringToBool(this string varString) => bool.Parse(varString);
    public static string GetOnlyNumbers(this string text)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(text))
            return string.Empty;

        var numbers = text.Where(char.IsDigit).ToArray();
        if (GuardClauseExtension.IsNull(numbers) || numbers.Length == 0)
            return string.Empty;

        return new string(numbers);
    }
    public static string TransformStringToDate(this string value)
    {
        return DateTime.TryParse(value, out var date) ? date.ToString("yyyy-MM-dd") : value;
    }
    public static string RemoveSpecialCharacters(this string text)
    {
        string[] specialCharacters = { "=", ":", "%", "/" };
        string result = string.Empty;
        string partText = string.Empty;
        if (GuardClauseExtension.IsNullOrWhiteSpace(text) == false)
        {
            for (int i = 0; i < text.Length; i++)
            {
                partText = text.ApplySubString(i, 1);
                if (!specialCharacters.Contains(partText))
                    result += partText;
            }
            return result;
        }
        return text;
    }
    public static string StripHTML(this string input)
    {
        return Regex.Replace(input, "<.*?>", String.Empty);
    }
    public static string RemoveQuotationMarks(this string value)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(value) == false && value.EndsWith("\'") || value.EndsWith("\""))
            return value.ApplyReplace("\'", "").ApplyReplace("\"", "").ApplyTrim();
        return value;
    }
    public static string ApplySubString(this string source, int startIndex = 0, int maxLength = 0)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(source))
            return source;
        source = source.ApplyTrim();
        return source.Length <= maxLength ? source : source.Substring(startIndex, maxLength);
    }
    public static string FormatCnpj(this string texto)
    {
        return Convert.ToUInt64(texto).ToString(@"00\.000\.000\/0000\-00");
    }
    public static string FormatCpf(this string texto)
    {
        return Convert.ToUInt64(texto).ToString(@"000\.000\.000\-00");
    }
    public static string FormatStringBase64ToString(this string text)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(text));
    }

    #region Metodo para pegar o base64 do front (btoa ou atob) e faz o processo de conversão

    public static string EncodingString(this string toEncode)
    {
        byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(toEncode);
        return Convert.ToBase64String(bytes);
    }

    public static string DecodingString(this string toDecode)
    {
        byte[] bytes = Convert.FromBase64String(toDecode);
        return ASCIIEncoding.ASCII.GetString(bytes);
    }

    #endregion

    public static string FormatDateTimeToString(this DateTime? date) => date.HasValue ? date.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : string.Empty;

    public static string GetDescriptionByBoolean(this bool? status) => status.HasValue ?
                                                   (status.Value ? EnumChoice.Yes.GetDisplayShortName() : EnumChoice.No.GetDisplayShortName())
                                                   : EnumChoice.No.GetDisplayShortName();

    public static string GetDescriptionByBoolean(this bool status) => status ? EnumChoice.Yes.GetDisplayShortName() : EnumChoice.No.GetDisplayShortName();

    public static string ApplyReplaceToAll(string text, string textToTrade, string newText) => Regex.Replace(text, textToTrade, newText, RegexOptions.IgnoreCase);

    public static string GetInitialsByName(this string fullName)
    {
        if (GuardClauseExtension.IsNotNullOrWhiteSpace(fullName))
            return string.Empty;

        var names = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (names.Length == 0)
            return string.Empty;

        char firstInitial = names[0][0];

        if (names.Length == 1)
            return char.ToUpper(firstInitial).ToString();

        char lastInicial = names[names.Length - 1][0];

        return $"{char.ToUpper(firstInitial)}{char.ToUpper(lastInicial)}";
    }
}
