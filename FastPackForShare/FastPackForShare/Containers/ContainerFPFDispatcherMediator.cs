using FastPackForShare.DispatcherMediator;
using Microsoft.Extensions.DependencyInjection;

namespace FastPackForShare.Containers;

public static class ContainerFPFDispatcherMediator
{
    public static void AddDispatcherMediatorHandlers(this ServiceCollection services, string assemblyName)
    {
        var myAssembly = AppDomain.CurrentDomain.Load(assemblyName);

        services.Scan(scan => scan
                .FromAssemblies(myAssembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IDispatcherMediatorCommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Scan(scan => scan
        .FromAssemblies(myAssembly)
        .AddClasses(classes => classes.AssignableTo(typeof(IDispatcherMediatorQueryHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime());
    }
}
