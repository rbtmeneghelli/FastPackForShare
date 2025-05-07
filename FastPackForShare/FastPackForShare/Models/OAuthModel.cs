namespace FastPackForShare.Models;

public sealed record OAuthModel
{
    public string OAuthPolicyName { get; set; } = "OAuth";
    public string ClientId { get; set; } = "seu_client_id";
    public string ClientSecret { get; set; } = "seu_client_secret";
    public string CallbackPath { get; set; } = "/oauth/callback";
    public string AuthorizationEndpoint { get; set; } = "URL_do_endpoint_de_autorizacao";
    public string TokenEndpoint { get; set; } = "URL_do_endpoint_de_token";
    public bool SaveTokens { get; set; } = true;
}