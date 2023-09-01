using System;
using EventFlow.Cap.Options;
using EventFlow.Subscribers;
using Microsoft.Extensions.DependencyInjection;

namespace EventFlow.Cap.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventFlowCapPublisher(this IServiceCollection services, Action<CapPublisherConfiguration> configurePublisher = null)
    {
        services.Configure(configurePublisher ?? CapPublisherConfiguration.DefaultConfiguration);

        services.AddSingleton<ISubscribeSynchronousToAll, CapPublisherSubscriber>();

        return services;
    }
}