using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace FastPackForShare.Containers;

public static class ContainerFPFSApplication
{
    public static void RegisterApplications(this WebApplication app, string corsName)
    {
        app.UseCors(corsName);
        app.UseHangfireDashboard();
    }
}
