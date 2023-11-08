using System.Net.Http.Json;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BlockInfrastructure.Core.Test.Shared.Integrations;

public abstract class IntegrationsTestHelper : IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly BlockInfrastructureCoreWebApplicationFactory _webApplicationFactory;
    protected readonly HttpClient WebRequestClient;

    protected IntegrationsTestHelper(ContainerFixture containerFixture)
    {
        _webApplicationFactory = new BlockInfrastructureCoreWebApplicationFactory(containerFixture);
        _serviceScope = _webApplicationFactory.Services.CreateScope();
        WebRequestClient = _webApplicationFactory.CreateClient();
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
        _webApplicationFactory.Dispose();
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        return _serviceScope.ServiceProvider.GetRequiredService<T>();
    }

    protected async Task<(User, TokenResponse)> CreateAccountAsync()
    {
        var databaseContext = GetRequiredService<DatabaseContext>();
        var user = new User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "kangdroid@test.com",
            ProfilePictureImageUrl = null,
            CredentialList = new List<Credential>
            {
                new()
                {
                    CredentialId = Ulid.NewUlid().ToString(),
                    CredentialProvider = CredentialProvider.Google
                }
            }
        };
        databaseContext.Users.Add(user);
        await databaseContext.SaveChangesAsync();

        // Login
        var loginRequest = new LoginRequest
        {
            AuthenticationCode = $"{user.CredentialList.First().CredentialId}_{user.Email}",
            Provider = CredentialProvider.Google
        };
        var response = await WebRequestClient.PostAsJsonAsync("/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

        return (user, tokenResponse);
    }
}