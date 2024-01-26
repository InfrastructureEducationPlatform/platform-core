using System.Net;
using System.Net.Http.Headers;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class DeploymentControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName = "GET /deployment/{deploymentId}: GetDeploymentAsync는 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_GetDeploymentAsync_Returns_Unauthorized_When_User_Not_Authenticated()
    {
        // Let
        var deploymentId = Ulid.NewUlid().ToString();

        // Do
        var response = await WebRequestClient.GetAsync($"/deployment/{deploymentId}");

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /deployment/{deploymentId}: GetDeploymentAsync는 만약 배포를 찾을 수 없는 경우 404 NotFound를 반환합니다.")]
    public async Task Is_GetDeploymentAsync_Returns_NotFound_When_Deployment_Not_Found()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var deploymentId = Ulid.NewUlid().ToString();

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync($"/deployment/{deploymentId}");

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName =
        "GET /deployment/{deploymentId}: GetDeploymentAsync는 만약 배포를 찾을 수 있는 경우 200 OK와 함께 DeploymentLog를 반환합니다.")]
    public async Task Is_GetDeploymentAsync_Returns_DeploymentLog_When_Deployment_Found()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var sketch = await CreateSketchAsync(channel.Id);
        var deploymentLog = await CreateDeploymentLogAsync(sketch.Id);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync($"/deployment/{deploymentLog.Id}");

        // Check HTTP Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}