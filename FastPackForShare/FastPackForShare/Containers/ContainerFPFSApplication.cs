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
}
