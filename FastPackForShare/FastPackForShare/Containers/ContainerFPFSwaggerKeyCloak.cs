using FastPackForShare.Enums;
using FastPackForShare.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace FastPackForShare.Containers;

public static class ContainerFPFSwaggerKeyCloak
{
    /// <summary>
    /// Exemplo com protocolo OAuth 2.0 e KeyCloak
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterJwtAuthToken(this IServiceCollection services, SwaggerDocConfig swaggerDocConfig)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = swaggerDocConfig.SwaggerKeyCloakConfig.Url;
            options.Audience = swaggerDocConfig.SwaggerKeyCloakConfig.KeyCloakClientID;
            options.RequireHttpsMetadata = false; // Para testes locais
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
            };
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = swaggerDocConfig.Title, Version = swaggerDocConfig.Version });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(swaggerDocConfig.SwaggerKeyCloakConfig.AuthorizationUrl),
                        TokenUrl = new Uri(swaggerDocConfig.SwaggerKeyCloakConfig.TokenUrl),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "Login básico" },
                            { "profile", "Perfil do usuário" },
                            { "email", "E-mail do usuário" }
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new[] { "openid", "profile", "email" }
                }
            });

            options.OperationFilter<AuthOperationFilter>();

            options.MapType<EnumStatus>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(typeof(EnumStatus)).Select(x => (IOpenApiAny)new OpenApiString(x)).ToList()
            });
        });
    }

    public static void UseSwaggerJwtAuthToken(this IApplicationBuilder builder, SwaggerDocConfig swaggerDocConfig)
    {
        builder.UseSwagger();
        builder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", swaggerDocConfig.Title);
            options.OAuthClientId(swaggerDocConfig.SwaggerKeyCloakConfig.OAuthClientId);
            options.OAuthClientSecret(swaggerDocConfig.SwaggerKeyCloakConfig.OAuthClientSecret); // Do Keycloak
            options.OAuthUsePkce();
            options.OAuthAppName(swaggerDocConfig.SwaggerKeyCloakConfig.OAuthAppName);
        });
    }
}
