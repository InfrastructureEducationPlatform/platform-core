using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Internal.PluginConfigs;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Models.Requests;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class PluginControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName = "GET /channels/{channelId}/plugins/available: ListAvailablePlugins는 인증하지 않은 사용자에 대해 401을 반환합니다.")]
    public async Task Is_ListAvailablePlugins_Returns_401_For_Unauthorized_User()
    {
        // Let N/A
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        WebRequestClient.DefaultRequestHeaders.Clear();
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}/plugins/available");

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /channels/{channelId}/plugins/available: ListAvailablePlugins는 인증된 사용자에 대해 200을 반환합니다.")]
    public async Task Is_ListAvailablePlugins_Returns_200_For_Authorized_User()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}/plugins/available");

        // Check
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Check 403 Case for ListAvailablePlugins
    [Fact(DisplayName =
        "GET /channels/{channelId}/plugins/available: ListAvailablePlugins는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_ListAvailablePlugins_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        var (secondUser, secondToken) = await CreateAccountAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}/plugins/available");

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels/{channelId}/plugins/install: InstallPluginToChannel는 인증하지 않은 사용자에 대해 401을 반환합니다.")]
    public async Task Is_InstallPluginToChannel_Returns_401_For_Unauthorized_User()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        WebRequestClient.DefaultRequestHeaders.Clear();
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/plugins/install",
            new InstallPluginRequest
            {
                PluginId = "test",
                PluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            });

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "POST /channels/{channelId}/plugins/install: InstallPluginToChannel는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_InstallPluginToChannel_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        var (secondUser, secondToken) = await CreateAccountAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/plugins/install",
            new InstallPluginRequest
            {
                PluginId = "test",
                PluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            });

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels/{channelId}/plugins/install: InstallPluginToChannel는 플러그인을 채널에 설치합니다.")]
    public async Task Is_InstallPluginToChannel_Installs_Plugin_To_Channel()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        var plugin = new Plugin
        {
            Id = "aws-static",
            Name = "Amazon Static Credential Provider Plugin",
            Description = "Amazon Access Key를 사용하는 Credential Provider Plugin",
            SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new AwsStaticProviderConfig
            {
                AccessKey = "Access Key ID",
                SecretKey = "Access Secret Key",
                Region = "Default Region Code(i.e: ap-northeast-2)"
            })
        };
        var installPluginRequest = new InstallPluginRequest
        {
            PluginId = plugin.Id,
            PluginConfiguration = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/plugins/install", installPluginRequest);

        // Check
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}