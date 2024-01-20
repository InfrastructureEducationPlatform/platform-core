using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Services.Authentication;
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
        });
        base.ConfigureWebHost(builder);
    }
}