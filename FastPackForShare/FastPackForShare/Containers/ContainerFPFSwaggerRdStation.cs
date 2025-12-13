using FastPackForShare.Enums;
using FastPackForShare.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

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

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                Scheme = "Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
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
