using FastPackForShare.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FastPackForShare.Containers;

public static class ContainerFPFSwaggerOptional
{
    /// <summary>
    /// Caso utilizar esse metodo, não é necessario incluir o AddAuthentication quanto AddAuthorization novamente!
    /// </summary>
    /// <param name="services"></param>
    /// <param name="swaggerDocConfig"></param>
    public static void RegisterService(this IServiceCollection services, SwaggerDocConfig swaggerDocConfig)
    {
        services.AddAuthentication().AddBearerToken();
        services.AddAuthorization();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = swaggerDocConfig.Title, Version = swaggerDocConfig.Version });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "Token",
                In = ParameterLocation.Header,
                Description = swaggerDocConfig.Description,
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
        });
    }
}
