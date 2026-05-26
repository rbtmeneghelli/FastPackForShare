namespace FastPackForShare.Helpers;

public static class HelperDocumentValidation
{
    private static readonly int[] PesosDv1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] PesosDv2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    /// <summary>
    /// Esse metodo implementa uma validação robusta para CNPJ legados e novos, seguindo as regras oficiais da Receita Federal do Brasil.
    /// </summary>
    /// <param name="cnpj"></param>
    /// <returns></returns>
    public static bool CnpjValidation(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // 1. Sanitização (remove máscara)
        var clean = Regex.Replace(cnpj, "[^a-zA-Z0-9]", "").ToUpper();

        // 2. Tamanho fixo
        if (clean.Length != 14)
            return false;

        // 3. Charset permitido (hard validation)
        if (!Regex.IsMatch(clean, "^[A-Z0-9]{14}$"))
            return false;

        // 4. DV deve ser numérico (regra oficial)
        if (!char.IsDigit(clean[12]) || !char.IsDigit(clean[13]))
            return false;

        // 5. Bloqueio de sequências inválidas (ex: 000..., AAA..., 111...)
        if (IsInvalidSequence(clean))
            return false;

        var baseCnpj = clean.Substring(0, 12);
        var dvInformado = clean.Substring(12, 2);

        // 6. Cálculo dos DVs
        var dvCalculado = CalculateDv(baseCnpj);

        return dvInformado == dvCalculado;
    }

    private static string CalculateDv(string baseCnpj)
    {
        var valores = baseCnpj.Select(CharToValue).ToArray();

        int dv1 = Mod11(valores, PesosDv1);
        int dv2 = Mod11(valores.Concat(new[] { dv1 }).ToArray(), PesosDv2);

        return $"{dv1}{dv2}";
    }

    private static int Mod11(int[] valores, int[] pesos)
    {
        int soma = 0;

        for (int i = 0; i < pesos.Length; i++)
        {
            soma += valores[i] * pesos[i];
        }

        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    // ✔️ Regra oficial (serve para CNPJ antigo e novo)
    private static int CharToValue(char c)
    {
        return ((int)c) - 48;
    }

    private static bool IsInvalidSequence(string input)
    {
        // todos os caracteres iguais
        return input.All(c => c == input[0]);
    }
}
