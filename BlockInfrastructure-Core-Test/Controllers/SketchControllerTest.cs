using System.Net;
using System.Net.Http.Headers;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Test.Shared.Integrations;
using BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;
using Newtonsoft.Json;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class SketchControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName = "GET /channels/{channelId}/sketches: ListSketchesAsync는 인증하지 않은 사용자에 대해 401을 반환합니다.")]
    public async Task Is_ListSketchesAsync_Returns_401_For_Unauthorized_User()
    {
        // Let N/A
        // Do
        var response = await WebRequestClient.GetAsync("/channels/test/sketches");

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /channels/{channelId}/sketches: ListSketchesAsync는 채널에 적절한 권한이 없는 경우 403을 반환합니다.")]
    public async Task Is_ListSketchesAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}/sketches");

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "GET /channels/{channelId}/sketches: ListSketchesAsync는 채널에 적절한 권한이 있는 경우 200을 반환합니다.")]
    public async Task Is_ListSketchesAsync_Returns_200_For_User_With_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}/sketches");

        // Check Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Check Response Data
        var responseData = JsonConvert.DeserializeObject<List<SketchResponse>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(responseData);
        Assert.Empty(responseData);
    }
}