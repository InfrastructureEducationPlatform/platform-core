using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Test.Shared.Integrations;
using BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

    [Fact(DisplayName = "POST /channels/{channelId}/sketches: CreateSketchAsync는 인증하지 않은 사용자에 대해 401을 반환합니다.")]
    public async Task Is_CreateSketchAsync_Returns_401_For_Unauthorized_User()
    {
        // Let
        var request = new CreateChannelRequest
        {
            Name = "Test",
            Description = "Test"
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/channels/test/sketches", request);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels/{channelId}/sketches: CreateSketchAsync는 채널에 적절한 권한이 없는 경우 403을 반환합니다.")]
    public async Task Is_CreateSketchAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();
        var request = new CreateChannelRequest
        {
            Name = "Test",
            Description = "Test"
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.PostAsJsonAsync("/channels/test/sketches", request);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels/{channelId}/sketches: CreateSketchAsync는 채널에 적절한 권한이 있는 경우 200을 반환합니다.")]
    public async Task Is_CreateSketchAsync_Returns_200_For_User_With_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var request = new CreateChannelRequest
        {
            Name = "Test",
            Description = "Test"
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/sketches", request);

        // Check Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /channels/{channelId}/sketches/{sketchId}: UpdateSketchAsync는 인증하지 않은 사용자에 대해 401을 반환합니다.")]
    public async Task Is_UpdateSketchAsync_Returns_401_For_Unauthorized_User()
    {
        // Let
        var request = new UpdateSketchRequest
        {
            Name = "Test",
            Description = "test",
            BlockData = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        var response = await WebRequestClient.PutAsJsonAsync("/channels/test/sketches/test", request);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /channels/{channelId}/sketches/{sketchId}: UpdateSketchAsync는 채널에 적절한 권한이 없는 경우 403을 반환합니다.")]
    public async Task Is_UpdateSketchAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();
        var request = new UpdateSketchRequest
        {
            Name = "Test",
            Description = "test",
            BlockData = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}/sketches/asdf", request);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName =
        "PUT /channels/{channelId}/sketches/{sketchId}: UpdateSketchAsync는 만약 스케치를 찾을 수 없는 경우 NotFound를 반환합니다.")]
    public async Task Is_UpdateSketchAsync_Returns_NotFound_If_Sketch_Not_Found()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var request = new UpdateSketchRequest
        {
            Name = "Test",
            Description = "test",
            BlockData = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}/sketches/asdf", request);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /channels/{channelId}/sketches/{sketchId}: UpdateSketchAsync는 채널에 적절한 권한이 있는 경우 200을 반환합니다.")]
    public async Task Is_UpdateSketchAsync_Returns_200_For_User_With_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var sketch = await CreateSketchAsync(channel.Id);
        var request = new UpdateSketchRequest
        {
            Name = "Test",
            Description = "test",
            BlockData = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}/sketches/{sketch.Id}", request);

        // Check Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}