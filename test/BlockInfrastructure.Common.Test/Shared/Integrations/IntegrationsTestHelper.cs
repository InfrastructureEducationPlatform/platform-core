using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlockInfrastructure.Common.Test.Shared.Integrations;

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

    protected async Task<Channel> CreateChannelAsync(string jwt)
    {
        var databaseContext = GetRequiredService<DatabaseContext>();
        var channelRequest = new CreateChannelRequest
        {
            Name = "Test",
            Description = "Test",
            ImageUrl = null
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var response = await WebRequestClient.PostAsJsonAsync("/channels", channelRequest);
        response.EnsureSuccessStatusCode();

        return await databaseContext.Channels.SingleAsync();
    }

    protected async Task<(User, TokenResponse)> CreateAccountWithExpiredTokenAsync()
    {
        var databaseContext = GetRequiredService<DatabaseContext>();
        var jwtService = GetRequiredService<IJwtService>();
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

        var claimList = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new("name", user.Name),
            new("profileUrl", user.ProfilePictureImageUrl ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };
        var expiredToken = jwtService.GenerateJwt(claimList, DateTime.Now.AddHours(-1));
        return (user, new TokenResponse
        {
            Token = expiredToken,
            LoginResult = LoginResult.LoginSucceed,
            RefreshToken = ""
        });
    }

    protected async Task<Sketch> CreateSketchAsync(string channelId)
    {
        var databaseContext = GetRequiredService<DatabaseContext>();
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = channelId,
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        databaseContext.Sketches.Add(sketch);
        await databaseContext.SaveChangesAsync();

        return sketch;
    }

    protected async Task<PluginInstallation> CreatePluginInstallation(string channelId)
    {
        var pluginInstallation = new PluginInstallation
        {
            ChannelId = channelId,
            PluginId = "aws-static",
            PluginConfiguration = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        var databaseContext = GetRequiredService<DatabaseContext>();
        databaseContext.PluginInstallations.Add(pluginInstallation);
        await databaseContext.SaveChangesAsync();
        return pluginInstallation;
    }

    protected async Task<DeploymentLog> CreateDeploymentLogAsync(string sketchId, string channelId,
                                                                 string pluginInstallationId)
    {
        var databaseContext = GetRequiredService<DatabaseContext>();

        var deploymentLog = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            SketchId = sketchId,
            PluginInstallationId = pluginInstallationId,
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = channelId
        };
        databaseContext.DeploymentLogs.Add(deploymentLog);
        await databaseContext.SaveChangesAsync();

        return deploymentLog;
    }
}