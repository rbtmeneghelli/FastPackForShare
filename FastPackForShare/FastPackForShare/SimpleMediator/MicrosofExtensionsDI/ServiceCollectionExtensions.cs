﻿using FastPackForShare.Containers;
using Microsoft.Extensions.DependencyInjection;

namespace FastPackForShare.SimpleMediator.MicrosofExtensionsDI;

/// <summary>
/// Extensions to scan for MediatR handlers and registers them.
/// - Scans for any handler interface implementations and registers them as <see cref="ServiceLifetime.Transient"/>
/// - Scans for any <see cref="IRequestPreProcessor{TRequest}"/> and <see cref="IRequestPostProcessor{TRequest,TResponse}"/> implementations and registers them as transient instances
/// Registers <see cref="IMediator"/> as a transient instance
/// After calling AddMediatR you can use the container to resolve an <see cref="IMediator"/> instance.
/// This does not scan for any <see cref="IPipelineBehavior{TRequest,TResponse}"/> instances including <see cref="RequestPreProcessorBehavior{TRequest,TResponse}"/> and <see cref="RequestPreProcessorBehavior{TRequest,TResponse}"/>.
/// To register behaviors, use the <see cref="ServiceCollectionServiceExtensions.AddTransient(IServiceCollection,Type,Type)"/> with the open generic or closed generic types.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers handlers and mediator types from the specified assemblies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">The action used to configure the options</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSimpleMediatR(this IServiceCollection services,
        Action<SimpleMediatRServiceConfiguration> configuration)
    {
        var serviceConfig = new SimpleMediatRServiceConfiguration();

        configuration.Invoke(serviceConfig);

        return services.AddSimpleMediatR(serviceConfig);
    }

    /// <summary>
    /// Registers handlers and mediator types from the specified assemblies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration options</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSimpleMediatR(this IServiceCollection services,
        SimpleMediatRServiceConfiguration configuration)
    {
        if (!configuration.AssembliesToRegister.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");
        }

        ContainerFPFSSimpleMediator.SetGenericRequestHandlerRegistrationLimitations(configuration);

        ContainerFPFSSimpleMediator.AddMediatRClassesWithTimeout(services, configuration);

        ContainerFPFSSimpleMediator.AddRequiredServices(services, configuration);

        return services;
    }
}
