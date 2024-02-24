using System.Net;
using System.Net.Http.Headers;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class PluginControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName = "GET /plugins: ListAvailablePlugins는 인증하지 않은 사용자에 대해 401을 반환합니다.")]
    public async Task Is_ListAvailablePlugins_Returns_401_For_Unauthorized_User()
    {
        // Let N/A
        // Do
        var response = await WebRequestClient.GetAsync("/plugins");

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /plugins: ListAvailablePlugins는 인증된 사용자에 대해 200을 반환합니다.")]
    public async Task Is_ListAvailablePlugins_Returns_200_For_Authorized_User()
    {
        // Let
        var (user, token) = await CreateAccountAsync();

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync("/plugins");

        // Check
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}