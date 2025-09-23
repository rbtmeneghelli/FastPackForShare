namespace FastPackForShare.Extensions;

/// <summary>
/// Melhoria no processo de classes de extensão
/// ANTES DO NET 10 >> public static class XPTO { public static decimal ImpostoTotal(dynamic o) => o.Total * 1.1m; }
/// </summary>
extension Net10Extension
{
    public decimal ImpostoTotal(dynamic o) => o.Total * 1.1m;
}