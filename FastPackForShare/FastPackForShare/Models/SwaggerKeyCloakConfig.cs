namespace FastPackForShare.Models;

public record SwaggerKeyCloakConfig
{
    public string Url { get; set; }
    public string KeyCloakClientID { get; set; }
    public string AuthorizationUrl { get; set; }
    public string TokenUrl { get; set; }
    public string OAuthClientId { get; set; }
    public string OAuthClientSecret { get; set; }
    public string OAuthAppName { get; set; }
}
