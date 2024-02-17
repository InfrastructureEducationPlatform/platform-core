using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using MassTransit.Testing;
using Newtonsoft.Json;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class UserControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName = "GET /users/me: GetUsersDetailProjectionAsync는 만약 인증되지 않은 사용자가 요청을 보낸 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_GetUsersDetailProjectionAsync_Returns_401_When_No_Token()
    {
        // Let - N/A
        // Do
        var response = await WebRequestClient.GetAsync("/users/me");

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /users/me: GetUsersDetailProjectionAsync는 사용자가 정상적인 사용자인 경우 User Projection을 반환합니다.")]
    public async Task Is_GetUsersDetailProjectionAsync_Returns_UserProjection_With_200_Ok()
    {
        // Let
        var (users, tokenResponse) = await CreateAccountAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        // Do
        var response = await WebRequestClient.GetAsync("/users/me");

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Check Response Body
        var userProjection = JsonConvert.DeserializeObject<MeProjection>(await response.Content.ReadAsStringAsync());
        Assert.Equal(users.Id, userProjection.UserId);
        Assert.Equal(users.Email, userProjection.Email);
        Assert.Equal(users.Name, userProjection.Name);
        Assert.Equal(users.ProfilePictureImageUrl, userProjection.ProfilePictureImageUrl);
        Assert.Empty(userProjection.ChannelPermissionList);
    }

    [Fact(DisplayName =
        "POST /users/preferences: UpdateUserPreferenceAsync는 만약 인증되지 않은 사용자가 요청을 보낸 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_UpdateUserPreferenceAsync_Returns_401_When_No_Token()
    {
        // Let - N/A
        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/users/preferences", new UpdateUserPreferenceRequest
        {
            Name = "New Name",
            Email = "asdf",
            ProfilePictureImageUrl = null
        });

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /users/preferences: UpdateUserPreferenceAsync는 만약 정상적인 사용자가 요청을 보낸 경우 204 NoContent를 반환합니다.")]
    public async Task Is_UpdateUserPreferenceAsync_Returns_204_When_Valid_User()
    {
        // Let
        var (users, tokenResponse) = await CreateAccountAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        // Do
        var testHarness = GetRequiredService<ITestHarness>();
        var response = await WebRequestClient.PostAsJsonAsync("/users/preferences", new UpdateUserPreferenceRequest
        {
            Name = "New Name",
            Email = "asdf",
            ProfilePictureImageUrl = null
        });

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check MassTransit Consumed
        var consumedMessages = await testHarness.Consumed.Any(a => a.MessageType == typeof(UserStateModifiedEvent));
        Assert.True(consumedMessages);
    }

    [Fact(DisplayName = "GET /users/search: SearchUserAsync는 만약 인증되지 않은 사용자가 요청을 보낸 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_SearchUserAsync_Returns_401_When_No_Token()
    {
        // Let - N/A
        // Do
        var response = await WebRequestClient.GetAsync("/users/search?query=Test");

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /users/search: SearchUserAsync는 만약 정상적인 사용자가 요청을 보낸 경우 200 Ok를 반환합니다.")]
    public async Task Is_SearchUserAsync_Returns_200_When_Valid_User()
    {
        // Let
        var (users, tokenResponse) = await CreateAccountAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        // Do
        var response = await WebRequestClient.GetAsync($"/users/search?query={users.Email}");

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Check Response Body
        var userSearchResponse =
            JsonConvert.DeserializeObject<List<UserSearchResponse>>(await response.Content.ReadAsStringAsync());
        Assert.Single(userSearchResponse);
        Assert.Equal(users.Id, userSearchResponse.First().UserId);
        Assert.Equal(users.Email, userSearchResponse.First().UserEmail);
        Assert.Equal(users.Name, userSearchResponse.First().UserName);
        Assert.Equal(users.ProfilePictureImageUrl, userSearchResponse.First().ProfilePictureImageUrl);
    }
}