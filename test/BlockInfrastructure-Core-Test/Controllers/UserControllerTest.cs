using System.Net;
using System.Net.Http.Headers;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
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
}