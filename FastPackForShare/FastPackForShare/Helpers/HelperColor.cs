using System.Drawing;

namespace FastPackForShare.Helpers;

public static class HelperColor
{
    private static int GetRandomNumber(int value, int? minValue = null)
    {
        if (minValue.HasValue)
            return Random.Shared.Next(minValue.GetValueOrDefault(0), value);

        return Random.Shared.Next(value);
    }

    public static string GetColorHex(string[] arrColorUsed)
    {
        string color = string.Empty;
        while (true)
        {
            color = GetColorHex();
            if (arrColorUsed.Any(x => x != color))
                break;
        }

        return color;
    }

    public static string GetColorHex()
    {
        int r = GetRandomNumber(255, 0);
        int g = GetRandomNumber(255, 0);
        int b = GetRandomNumber(255, 0);
        int a = GetRandomNumber(255, 0);
        string cor = $"#{Color.FromArgb(a, r, g, b).Name.Trim().Substring(0, 6)}";
        return cor;
    }

    public static string GetColorRgb(string[] arrColorUsed)
    {
        string color = string.Empty;
        while (true)
        {
            color = GetColorRgb().ToString();
            if (arrColorUsed.Any(x => x != color))
                break;
        }

        return color;
    }

    public static Color GetColorRgb()
    {
        Color randomColor = Color.FromArgb(GetRandomNumber(256), GetRandomNumber(256), GetRandomNumber(256));
        return randomColor;
    }
}
