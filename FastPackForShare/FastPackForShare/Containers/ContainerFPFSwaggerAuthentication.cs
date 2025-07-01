using FastPackForShare.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FastPackForShare.Constants;
using FastPackForShare.Cryptography;
using FastPackForShare.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.Buffers.Text;
using FastPackForShare.Interfaces;

namespace FastPackForShare.Containers;

public static class ContainerFPFSwaggerAuthentication
{
    public static IServiceCollection RegisterAuthentication(this IServiceCollection services, JwtConfigModel tokenSettings)
    {
        services.AddAuthentication
              (x =>
              {
                  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
              .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ClockSkew = TimeSpan.Zero,
                      ValidIssuer = tokenSettings.Issuer,
                      ValidAudience = tokenSettings.Audience,
                      IssuerSigningKey = new SymmetricSecurityKey
                      (Encoding.UTF8.GetBytes(tokenSettings.Key))
                  };
              });

        return services;
    }

    public static void RegisterAuthenticationEncrypt(this IServiceCollection services, JwtConfigModel jwtConfigModel)
    {
        services.AddAuthentication
              (x =>
              {
                  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
              .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ClockSkew = TimeSpan.Zero,
                      ValidIssuer = jwtConfigModel.Issuer,
                      ValidAudience = jwtConfigModel.Audience,
                      IssuerSigningKey = new SymmetricSecurityKey
                      (Encoding.UTF8.GetBytes(jwtConfigModel.Key))
                  };
                  options.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = async context =>
                      {
                          var endpoint = context.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint;
                          if (endpoint != null && endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null)
                          {
                              return;
                          }

                          else if (context.Request.Headers.TryGetValue("Authorization", out var tokenHeader))
                          {
                              var iTokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                              var tokenAuthAPI = StringExtension.ApplyReplaceToAll(tokenHeader.ToString(), "Bearer ", "");

                              if (GuardClauseExtension.IsNullOrWhiteSpace(tokenAuthAPI))
                              {
                                  return;
                              }

                              else if (Base64.IsValid(tokenAuthAPI))
                              {
                                  try
                                  {
                                      string tokenDecrypt = CryptographyHashTokenManager.DecryptToken(tokenAuthAPI, jwtConfigModel.Key);
                                      if (iTokenService.ValidateTokenAuthentication(tokenDecrypt))
                                          context.Token = tokenDecrypt;
                                  }
                                  catch
                                  {
                                      return;
                                  }
                              }
                          }

                          await Task.CompletedTask;
                          return;
                      },
                      OnChallenge = async context =>
                      {
                          if (context.Error == "invalid_token" || context.Error == "missing_token")
                          {
                              context.HandleResponse();
                              context.Response.StatusCode = ConstantHttpStatusCode.FORBIDDEN_CODE;
                              context.Response.ContentType = "application/json";
                              var responseForbidden = new
                              {
                                  sucesso = false,
                                  mensagem = ConstantMessageResponse.FORBIDDEN_CODE
                              };
                              await context.Response.WriteAsJsonAsync(responseForbidden);
                              return;
                          }

                          context.HandleResponse();
                          context.Response.StatusCode = ConstantHttpStatusCode.UNAUTHORIZED_CODE;
                          context.Response.ContentType = "application/json";
                          var response = new
                          {
                              sucesso = false,
                              mensagem = ConstantMessageResponse.UNAUTHORIZED_CODE
                          };
                          await context.Response.WriteAsJsonAsync(response);
                          return;
                      }
                  };
              });

        services.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                .RequireAuthenticatedUser()
                .Build());

            auth.AddPolicy("BearerRole", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim("Admin")
                .Build());
        });
    }
}
