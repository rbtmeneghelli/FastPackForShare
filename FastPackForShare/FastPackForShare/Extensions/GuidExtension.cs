namespace FastPackForShare.Extensions;

/// <summary>
/// Formatos do GUID
/// D: 32 dígitos, mas com os hífens. Esse é o padrão;
/// N: 32 dígitos, sem outros símbolos; (Ideal para usar em nome de arquivos)
/// B: Usa os hífens e a string é envolta entre chaves
/// P: semelhante a B, mas com parênteses em vez de chaves
/// X:  Usa a representação hexadecimal do guid.
/// </summary>
public static class GuidExtension
{
    public static string GetGuidDigits(string typeGuid) => Guid.NewGuid().ToString(typeGuid);
    public static Guid GetGuidDigitsByString(this string text) => Guid.Parse(text);

    /// <summary>
    /// Metodo para geração de UUID mais atualizado que o Guid Padrão
    /// </summary>
    /// <param name="timeProvider">Formatação de data e Hora</param>
    /// <returns></returns>
    //public static Guid GenerateVersion7Uuid(ITimeProvider timeProvider) => Guid.CreateVersion7(timeProvider.GetUtcNow());
}
