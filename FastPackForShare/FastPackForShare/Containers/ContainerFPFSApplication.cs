using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace FastPackForShare.Containers;

public static class ContainerFPFSApplication
{
    public static void RegisterApplications(this WebApplication app)
    {
        app.UseCors("APICORS");
        app.UseHangfireDashboard();
    }
}
