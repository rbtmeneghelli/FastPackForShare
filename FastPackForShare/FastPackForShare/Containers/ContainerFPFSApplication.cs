using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace FastPackForShare.Containers;

public static class ContainerFPFSApplication
{
    public static void RegisterCors(this WebApplication app, string corsName)
    {
        app.UseCors(corsName);
    }

    public static void RegisterHangfire(this WebApplication app, string corsName)
    {
        app.UseHangfireDashboard();
    }

    public static void RegisterGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler();
    }

    public static void RegisterSecurityHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Content-Security-Policy"] = "default-src 'self'";
            headers["X-Frame-Options"] = "DENY";
            await next();
        });
    }

    public static void RegisterHsts(this WebApplication app)
    {
        app.UseHsts();
    }

    public static void RegisterHttpsRedirection(this WebApplication app)
    {
        app.UseHttpsRedirection();
    }

    public static void RegisterAntiForgery(this WebApplication app)
    {
        app.UseAntiforgery();
    }

    public static void RegisterRateLimiter(this WebApplication app)
    {
        app.UseRateLimiter();
    }
}