using FastPackForShare.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FastPackForShare.Containers;

public static class ContainerFPFSwaggerDefault
{
    public static void RegisterService(this IServiceCollection services, SwaggerDocConfig swaggerDocConfig)
    {
        services.AddSwaggerGen(
        c =>
        {
            c.SwaggerDoc(swaggerDocConfig.Version, new OpenApiInfo
            {
                Title = swaggerDocConfig.Title,
                Version = swaggerDocConfig.Version,
                Description = swaggerDocConfig.Description,
                Contact = new OpenApiContact() { Name = swaggerDocConfig.Contact.Name, Email = swaggerDocConfig.Contact.Email },
                License = new OpenApiLicense() { Name = swaggerDocConfig.License.Name, Url = swaggerDocConfig.License.Url }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                Scheme = "Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                          Type = ReferenceType.SecurityScheme,
                          Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WebAPI.xml"));
        });
    }

    public static void RegisterApplication(this IApplicationBuilder applicationBuilder, SwaggerDocConfig swaggerDocConfig)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/V1/swagger.json", swaggerDocConfig.Title);
            c.InjectStylesheet("/Arquivos/swagger-dark.css");
        });
    }
}
