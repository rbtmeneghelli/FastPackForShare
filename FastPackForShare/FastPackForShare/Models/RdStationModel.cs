using FastPackForShare.Enums;
using System.Text.Json.Serialization;

namespace FastPackForShare.Models;

public record RdStationConfigModel
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Code { get; set; }
}

public record LeadModel
{
    public string Email { get; set; }
    public string Name { get; set; }
}

public sealed record RdStationResult
{
    public int Code { get; set; }
    public object Data { get; set; }
    public string Message { get; set; }
}

public sealed record RdStationReqDto
{
    public string URL { get; set; } = string.Empty;
    public object PayLoad { get; set; } = new();
    public EnumRdStationAutentication EnumRdStationAutentication { get; set; } = EnumRdStationAutentication.AUTENTICACAO_BEARERTOKEN;
}

public sealed record RdStationRespTokenDto
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; }
    [JsonPropertyName("expires_in")]
    public double SecondsToExpire { get; set; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    public DateTime? ExpirationTokenDate { get; set; }
}