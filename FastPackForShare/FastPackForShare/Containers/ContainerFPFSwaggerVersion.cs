using FastPackForShare.Constants;
using FastPackForShare.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FastPackForShare.Containers;

public static class ContainerFPFSwaggerVersion
{
    public static void RegisterService(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();

            c.AddSecurityDefinition("Bearer", GetBearerConfig());

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = GetSecurityConfig()
                    },
                    new string[] {}
                }
            });
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    }

    public static void RegisterApplication(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    options.InjectStylesheet("/Arquivos/swagger-dark.css");
                }
            });
    }

    private static OpenApiSecurityScheme GetBearerConfig()
    {
        return new OpenApiSecurityScheme
        {
            Description = "Insira o token JWT desta maneira: Bearer {seu token}",
            Name = "Authorization",
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        };
    }

    private static OpenApiReference GetSecurityConfig()
    {
        return new OpenApiReference()
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        };
    }
}

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, GetApiConfig(description));
            // Apresentar documentação mais detalhe do Swagger
            // options.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "WebAPISwagger.xml"));
        }
    }

    private static OpenApiInfo GetApiConfig(ApiVersionDescription description)
    {
        return new OpenApiInfo
        {
            Title = "API - Default",
            Version = description.ApiVersion.ToString(),
            Description = description.IsDeprecated ? " Esta versão está obsoleta!" : "Lista de endpoints disponíveis",
            Contact = new OpenApiContact() { Name = "Dev", Email = "dev@test.com.br" },
            License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        int[] arrHttpStatusCode = [
            ConstantHttpStatusCode.BAD_REQUEST_CODE,
        ConstantHttpStatusCode.UNAUTHORIZED_CODE,
        ConstantHttpStatusCode.FORBIDDEN_CODE,
        ConstantHttpStatusCode.INTERNAL_ERROR_CODE
        ];

        for (var i = 0; i <= arrHttpStatusCode.Length - 1; i++)
            operation.Responses.Remove(arrHttpStatusCode[i].ToString());

        operation.Responses.Add(ConstantHttpStatusCode.BAD_REQUEST_CODE.ToString(), new OpenApiResponse { Description = ConstantMessageResponse.BAD_REQUEST_CODE, });
        operation.Responses.Add(ConstantHttpStatusCode.UNAUTHORIZED_CODE.ToString(), new OpenApiResponse { Description = ConstantMessageResponse.UNAUTHORIZED_CODE, });
        operation.Responses.Add(ConstantHttpStatusCode.FORBIDDEN_CODE.ToString(), new OpenApiResponse { Description = ConstantMessageResponse.FORBIDDEN_CODE });
        operation.Responses.Add(ConstantHttpStatusCode.INTERNAL_ERROR_CODE.ToString(), new OpenApiResponse { Description = ConstantMessageResponse.INTERNAL_ERROR_CODE });
        operation.Deprecated = context.ApiDescription.IsDeprecated() ? true : OpenApiOperation.DeprecatedDefault;

        if (GuardClauseExtension.IsNull(operation.Parameters))
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = context.ApiDescription
                .ParameterDescriptions
                .First(p => p.Name == parameter.Name);

            var routeInfo = description.RouteInfo;

            operation.Deprecated = context.ApiDescription.IsDeprecated() ? true : OpenApiOperation.DeprecatedDefault;

            if (GuardClauseExtension.IsNull(parameter.Description))
            {
                parameter.Description = description.ModelMetadata?.Description;
            }

            if (GuardClauseExtension.IsNull(routeInfo))
            {
                continue;
            }

            if (parameter.In != ParameterLocation.Path && GuardClauseExtension.IsNull(parameter.Schema.Default))
            {
                parameter.Schema.Default = new OpenApiString(routeInfo.DefaultValue.ToString());
            }

            parameter.Required |= !routeInfo.IsOptional;
        }
    }
}
