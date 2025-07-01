using Microsoft.OpenApi.Models;

namespace FastPackForShare.Models;

public record SwaggerDocConfig
{
    public string Title { get; set; } = "API - Default";
    public string Version { get; set; } = "V1";
    public string Description { get; set; } = "Lista de endpoints disponíveis";
    public OpenApiContact Contact { get; set; } = new();
    public OpenApiLicense License { get; set; } = new();
    public SwaggerKeyCloakConfig SwaggerKeyCloakConfig { get; set; } = new();
}
