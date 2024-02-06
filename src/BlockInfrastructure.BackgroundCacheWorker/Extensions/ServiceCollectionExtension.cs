using System.Reflection;
using BlockInfrastructure.BackgroundCacheWorker.Consumers.Channels;
using BlockInfrastructure.BackgroundCacheWorker.Consumers.User;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using IBusRegistrationConfigurator = MassTransit.IBusRegistrationConfigurator;
using IBusRegistrationContext = MassTransit.IBusRegistrationContext;
using IRabbitMqBusFactoryConfigurator = MassTransit.IRabbitMqBusFactoryConfigurator;

namespace BlockInfrastructure.BackgroundCacheWorker.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBackgroundCacheWorker(this IServiceCollection services)
    {
        return services;
    }

    public static IBusRegistrationConfigurator RegisterBackgroundCacheConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<ResetMeProjectionCacheConsumer>();
        configurator.AddConsumer<ResetChannelInformationCacheConsumer>();
        return configurator;
    }

    public static IRabbitMqBusFactoryConfigurator ConfigureBackgroundCacheEndpoint(
        this IRabbitMqBusFactoryConfigurator configurator,
        IBusRegistrationContext ctx)
    {
        configurator.ReceiveEndpoint("user.modified.invalidate-me", cfg =>
        {
            // Setup Consumer
            cfg.Bind("user.modified");
            cfg.ConfigureConsumer<ResetMeProjectionCacheConsumer>(ctx);
        });

        configurator.ReceiveEndpoint("channel.modified.invalidate-channel-information", cfg =>
        {
            // Setup Consumer
            cfg.Bind("channel.modified");
            cfg.ConfigureConsumer<ResetChannelInformationCacheConsumer>(ctx);
        });
        return configurator;
    }

    public static MediatRServiceConfiguration ConfigureMediatRBackgroundCache(this MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        return configuration;
    }
}