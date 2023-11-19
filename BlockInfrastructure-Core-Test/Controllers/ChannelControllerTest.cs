using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Test.Shared.Integrations;
using BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class ChannelControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName =
        "POST /channels: CreateChannelAsync는 만약 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_CreateChannelAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Let
        var request = new CreateChannelRequest
        {
            Name = Ulid.NewUlid().ToString(),
            Description = Ulid.NewUlid().ToString(),
            ImageUrl = null
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/channels", request);

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels: CreateChannelAsync는 만약 만료된 토큰을 가진 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_CreateChannelAsync_Returns_401_Unauthorized_When_Expired_Token_Requested()
    {
        // Let
        var (user, expiredToken) = await CreateAccountWithExpiredTokenAsync();
        var request = new CreateChannelRequest
        {
            Name = Ulid.NewUlid().ToString(),
            Description = Ulid.NewUlid().ToString(),
            ImageUrl = null
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken.Token);

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/channels", request);

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels: CreateChannelAsync는 만약 정상적인 요청인 경우 채널을 생성하고 204 NoContent를 반환합니다.")]
    public async Task Is_CreateChannelAsync_Returns_NoContent_When_Channel_Created()
    {
        // Let
        var (user, tokenResponse) = await CreateAccountAsync();
        var request = new CreateChannelRequest
        {
            Name = "KangDroid",
            Description = "KangDroid",
            ImageUrl = null
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/channels", request);

        // Check HTTP Status Code
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check Channel Data
        var databaseContext = GetRequiredService<DatabaseContext>();
        var channel = await databaseContext.Channels.SingleAsync();
        Assert.Equal(request.Name, channel.Name);
        Assert.Equal(request.Description, channel.Description);

        // Check Ownership
        var ownership = await databaseContext.ChannelPermissions.SingleAsync();
        Assert.Equal(user.Id, ownership.UserId);
        Assert.Equal(ChannelPermissionType.Owner, ownership.ChannelPermissionType);
    }
}