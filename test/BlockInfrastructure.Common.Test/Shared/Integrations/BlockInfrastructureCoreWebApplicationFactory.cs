using BlockInfrastructure.BackgroundCacheWorker.Extensions;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Services.Authentication;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockInfrastructure.Common.Test.Shared.Integrations;

internal class BlockInfrastructureCoreWebApplicationFactory(ContainerFixture containerFixture) : WebApplicationFactory<Program>
{
    public readonly IConfiguration Configuration = TestConfiguration.Create(containerFixture);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.Sources.Clear();
            configurationBuilder.AddConfiguration(Configuration);
        });
        builder.UseConfiguration(Configuration);
        builder.ConfigureTestServices(conf =>
        {
            // Replace OAuthProviderService
            conf.AddSingleton<IntegrationSelfAuthenticationProvider>();
            conf.AddSingleton<AuthProviderServiceFactory>(serviceProvider => _ =>
                serviceProvider.GetRequiredService<IntegrationSelfAuthenticationProvider>());

            // Remove MassTransit Hosted Service
            conf.RemoveMassTransitHostedService();

            // Add TestHarness
            conf.AddMassTransitTestHarness(configurator =>
            {
                configurator.RegisterBackgroundCacheConsumers();
                configurator.UsingRabbitMq((ctx, busFactoryConfigurator) =>
                {
                    // MassTransit Default
                    busFactoryConfigurator.Host(Configuration["RabbitMq:Host"],
                        Convert.ToUInt16(Configuration["RabbitMq:Port"]), "/", h =>
                        {
                            h.Username(Configuration["RabbitMq:Username"]);
                            h.Password(Configuration["RabbitMq:Password"]);
                        });

                    // Configure Message
                    busFactoryConfigurator.Message<UserStateModifiedEvent>(a => a.SetEntityName("user.modified"));

                    // Configure Cache Consumer(Worker)
                    busFactoryConfigurator.ConfigureBackgroundCacheEndpoint(ctx);
                });
            });
        });
        base.ConfigureWebHost(builder);
    }
}