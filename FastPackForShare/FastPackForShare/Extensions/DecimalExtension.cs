namespace FastPackForShare.Extensions;

public static class DecimalExtensionMethods
{
    public static decimal Sum(this decimal input, decimal value) => decimal.Add(input, value);
    public static decimal Subtract(this decimal input, decimal value) => decimal.Subtract(input, value);
    public static decimal Multiply(this decimal input, decimal value) => decimal.Multiply(input, value);
    public static decimal ModDivision(this decimal input, decimal value) => decimal.Remainder(input, value);
    public static decimal Round(this decimal input, int decimals = 2) => decimal.Round(input, decimals);
    public static decimal Divide(this decimal input, decimal value) => decimal.Divide(input, value);
    public static decimal GetMaxValue(decimal firstValue, decimal secondValue) => decimal.Max(firstValue, secondValue);
    public static decimal GetMinValue(decimal firstValue, decimal secondValue) => decimal.Min(firstValue, secondValue);

    extension(decimal)
    {
        /* Membro estático associado ao tipo */
        public static string ConvertDecimalToString(decimal value) => value.ToString("N");  
    }
}

