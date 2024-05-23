using System.Net;
using System.Net.Http.Headers;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using Xunit.Abstractions;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class DeploymentControllerTest(ContainerFixture containerFixture, ITestOutputHelper outputHelper) : IntegrationsTestHelper(containerFixture, outputHelper)
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
        var pluginInstallation = await CreatePluginInstallation(channel.Id);
        var deploymentLog = await CreateDeploymentLogAsync(sketch.Id, channel.Id, pluginInstallation.Id);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync($"/deployment/{deploymentLog.Id}");

        // Check HTTP Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "GET /deployment: GetDeploymentInformationListAsync는 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_GetDeploymentInformationListAsync_Returns_Unauthorized_When_User_Not_Authenticated()
    {
        // Let - N/A
        // Do
        var response = await WebRequestClient.GetAsync("/deployment");

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "GET /deployment: GetDeploymentInformationListAsync는 인증된 사용자가 요청한 경우 200 OK와 함께 DeploymentLog 리스트를 반환합니다.")]
    public async Task Is_GetDeploymentInformationListAsync_Returns_DeploymentLogList_When_User_Authenticated()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var sketch = await CreateSketchAsync(channel.Id);
        var pluginInstallation = await CreatePluginInstallation(channel.Id);
        var deploymentLog = await CreateDeploymentLogAsync(sketch.Id, channel.Id, pluginInstallation.Id);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync("/deployment");

        // Check HTTP Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName =
        "DELETE /deployment/{deploymentId}: DeleteDeploymentAsync는 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_DeleteDeploymentAsync_Returns_Unauthorized_When_User_Not_Authenticated()
    {
        // Let
        var deploymentId = Ulid.NewUlid().ToString();

        // Do
        var response = await WebRequestClient.DeleteAsync($"/deployment/{deploymentId}");

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "DELETE /deployment/{deploymentId}: DeleteDeploymentAsync는 만약 배포를 찾을 수 없는 경우 404 NotFound를 반환합니다.")]
    public async Task Is_DeleteDeploymentAsync_Returns_NotFound_When_Deployment_Not_Found()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var deploymentId = Ulid.NewUlid().ToString();

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.DeleteAsync($"/deployment/{deploymentId}");

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName =
        "DELETE /deployment/{deploymentId}: DeleteDeploymentAsync는 만약 삭제하려고 하는 배포가 최신이 아닌 경우 400 BadRequest를 반환합니다.")]
    public async Task Is_DeleteDeploymentAsync_Returns_BadRequest_When_Deployment_Not_Latest()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var sketch = await CreateSketchAsync(channel.Id);
        var pluginInstallation = await CreatePluginInstallation(channel.Id);
        var deploymentLog = await CreateDeploymentLogAsync(sketch.Id, channel.Id, pluginInstallation.Id);
        var deploymentLog2 = await CreateDeploymentLogAsync(sketch.Id, channel.Id, pluginInstallation.Id);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.DeleteAsync($"/deployment/{deploymentLog.Id}");

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /deployment/{deploymentId}: DeleteDeploymentAsync는 만약 배포를 찾을 수 있는 경우 204 NoContent를 반환합니다.")]
    public async Task Is_DeleteDeploymentAsync_Returns_NoContent_When_Deployment_Found()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var sketch = await CreateSketchAsync(channel.Id);
        var pluginInstallation = await CreatePluginInstallation(channel.Id);
        var deploymentLog = await CreateDeploymentLogAsync(sketch.Id, channel.Id, pluginInstallation.Id);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.DeleteAsync($"/deployment/{deploymentLog.Id}");

        // Check HTTP Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}