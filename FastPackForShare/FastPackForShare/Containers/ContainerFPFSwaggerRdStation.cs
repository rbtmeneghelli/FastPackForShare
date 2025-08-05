using FastPackForShare.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FastPackForShare.Containers;

public static class ContainerFPFSwaggerRdStation
{
    public static void RegisterService(this IServiceCollection services, SwaggerDocConfig swaggerDocConfig)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(swaggerDocConfig.Version, new OpenApiInfo
            {
                Title = swaggerDocConfig.Title,
                Version = swaggerDocConfig.Version,
                Description = swaggerDocConfig.Description,
                Contact = new OpenApiContact() { Name = swaggerDocConfig.Contact.Name, Email = swaggerDocConfig.Contact.Email },
                License = new OpenApiLicense() { Name = swaggerDocConfig.License.Name, Url = swaggerDocConfig.License.Url }
            });

            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Insira o token JWT no campo abaixo. Exemplo: Bearer {seu_token}"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            options.OperationFilter<AuthOperationFilter>();
        });
    }

    public static void RegisterApplication(this IApplicationBuilder applicationBuilder, SwaggerDocConfig swaggerDocConfig)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI();
    }
}
