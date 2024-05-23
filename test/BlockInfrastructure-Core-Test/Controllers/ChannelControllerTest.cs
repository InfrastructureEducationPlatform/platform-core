using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class ChannelControllerTest(ContainerFixture containerFixture, ITestOutputHelper outputHelper) : IntegrationsTestHelper(containerFixture, outputHelper)
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

        // Check Message Consumed
        var testHarness = GetRequiredService<ITestHarness>();
        var consumedMessages = await testHarness.Consumed.Any(a => a.MessageType == typeof(ChannelStateModifiedEvent));
        Assert.True(consumedMessages);
    }

    [Fact(DisplayName =
        "GET /channels/{channelId}: GetChannelInformationAsync는 만약 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_GetChannelInformationAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();

        // Do
        var response = await WebRequestClient.GetAsync($"/channels/{channelId}");

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /channels/{channelId}: GetChannelInformationAsync는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_GetChannelInformationAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}");

        // Check
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "GET /channels/{channelId}: GetChannelInformationAsync는 만약 채널이 존재하는 경우 채널 정보를 반환합니다.")]
    public async Task Is_GetChannelInformationAsync_Returns_Channel_Information_When_Channel_Exists()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        var response = await WebRequestClient.GetAsync($"/channels/{channel.Id}");

        // Check
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var channelInformation = await response.Content.ReadFromJsonAsync<ChannelInformationResponse>();
        Assert.Equal(channel.Id, channelInformation.ChannelId);
        Assert.Equal(channel.Name, channelInformation.Name);
        Assert.Equal(channel.Description, channelInformation.Description);
    }

    [Fact(DisplayName =
        "PUT /channels/{channelId}: UpdateChannelInformationAsync는 만약 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_UpdateChannelInformationAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var request = new UpdateChannelInformationRequest
        {
            ChannelName = "KangDroid",
            ChannelDescription = "KangDroid",
            ProfileImageUrl = null
        };

        // Do
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channelId}", request);

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "PUT /channels/{channelId}: UpdateChannelInformationAsync는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_UpdateChannelInformationAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();
        var request = new UpdateChannelInformationRequest
        {
            ChannelName = "KangDroid",
            ChannelDescription = "KangDroid",
            ProfileImageUrl = null
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}", request);

        // Check
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /channels/{channelId}: UpdateChannelInformationAsync는 채널 정보를 수정합니다.")]
    public async Task Is_UpdateChannelInformationAsync_Updates_Channel_Information()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var request = new UpdateChannelInformationRequest
        {
            ChannelName = "KangDroid",
            ChannelDescription = "KangDroid",
            ProfileImageUrl = null
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}", request);

        // Check
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check Data
        var databaseContext = GetRequiredService<DatabaseContext>();
        var updatedChannel = await databaseContext.Channels.AsNoTracking().SingleAsync();
        Assert.Equal(request.ChannelName, updatedChannel.Name);
        Assert.Equal(request.ChannelDescription, updatedChannel.Description);
    }

    [Fact(DisplayName =
        "PUT /channels/{channelId}/users: UpdateUserChannelRoleAsync는 만약 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_UpdateUserChannelRoleAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var request = new UpdateUserChannelRoleRequest
        {
            UserId = Ulid.NewUlid().ToString(),
            ChannelPermissionType = ChannelPermissionType.Owner
        };

        // Do
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channelId}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "PUT /channels/{channelId}/users: UpdateUserChannelRoleAsync는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_UpdateUserChannelRoleAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();
        var request = new UpdateUserChannelRoleRequest
        {
            UserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName =
        "PUT /channels/{channelId}/users: UpdateUserChannelRoleAsync는 만약 현재 요청하는 사람이 직접 수정하려고 하는 경우 400 BadRequest를 반환합니다.")]
    public async Task Is_UpdateUserChannelRoleAsync_Returns_400_BadRequest_When_User_Tries_To_Update_Itself()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var request = new UpdateUserChannelRoleRequest
        {
            UserId = user.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "PUT /channels/{channelId}/users: UpdateUserChannelRoleAsync는 채널 권한 정보를 수정합니다.")]
    public async Task Is_UpdateUserChannelRoleAsync_Updates_Channel_Permission_Well()
    {
        // Let
        var databaseContext = GetRequiredService<DatabaseContext>();
        var (user, token) = await CreateAccountAsync();
        var (secondUser, secondToken) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var permission = new ChannelPermission
        {
            ChannelId = channel.Id,
            UserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Reader
        };
        databaseContext.ChannelPermissions.Add(permission);
        await databaseContext.SaveChangesAsync();
        var request = new UpdateUserChannelRoleRequest
        {
            UserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.PutAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check Data
        var updatedChannelPermission = await databaseContext.ChannelPermissions
                                                            .AsNoTracking()
                                                            .Where(a => a.ChannelId == channel.Id && a.UserId == secondUser.Id)
                                                            .SingleAsync();
        Assert.Equal(request.UserId, updatedChannelPermission.UserId);
        Assert.Equal(request.ChannelPermissionType, updatedChannelPermission.ChannelPermissionType);
    }

    [Fact(DisplayName =
        "DELETE /channels/{channelId}/users/{userId}: RemoveUserFromChannelAsync는 만약 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_RemoveUserFromChannelAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var userId = Ulid.NewUlid().ToString();

        // Do
        var response = await WebRequestClient.DeleteAsync($"/channels/{channelId}/users/{userId}");

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "DELETE /channels/{channelId}/users/{userId}: RemoveUserFromChannelAsync는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_RemoveUserFromChannelAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.DeleteAsync($"/channels/{channel.Id}/users/{secondUser.Id}");

        // Check
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName =
        "DELETE /channels/{channelId}/users/{userId}: RemoveUserFromChannelAsync는 본인이 채널에서 나가려고 하는 경우 400 BadRequest를 반환합니다.")]
    public async Task Is_RemoveUserFromChannelAsync_Returns_400_BadRequest_When_User_Tries_To_Remove_Itself()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        var response = await WebRequestClient.DeleteAsync($"/channels/{channel.Id}/users/{user.Id}");

        // Check
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "DELETE /channels/{channelId}/users/{userId}: RemoveUserFromChannelAsync는 채널 권한 정보를 삭제합니다.")]
    public async Task Is_RemoveUserFromChannelAsync_Removes_Channel_Permission_Well()
    {
        // Let
        var databaseContext = GetRequiredService<DatabaseContext>();
        var (user, token) = await CreateAccountAsync();
        var (secondUser, secondToken) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var permission = new ChannelPermission
        {
            ChannelId = channel.Id,
            UserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Reader
        };
        databaseContext.ChannelPermissions.Add(permission);
        await databaseContext.SaveChangesAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.DeleteAsync($"/channels/{channel.Id}/users/{secondUser.Id}");

        // Check
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check Data
        var removedChannelPermission = await databaseContext.ChannelPermissions
                                                            .AsNoTracking()
                                                            .Where(a => a.ChannelId == channel.Id && a.UserId == secondUser.Id)
                                                            .SingleOrDefaultAsync();
        Assert.Null(removedChannelPermission);
    }

    [Fact(DisplayName =
        "POST /channels/{channelId}/users: AddUserToChannelAsync는 만약 요청하는 본인을 추가하려고 하는 경우 400 BadRequest를 반환합니다.")]
    public async Task Is_AddUserToChannelAsync_Returns_400_BadRequest_When_User_Tries_To_Add_Itself()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var request = new AddUserToChannelRequest
        {
            TargetUserId = user.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName =
        "POST /channels/{channelId}/users: AddUserToChannelAsync는 만약 인증되지 않은 사용자가 요청한 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_AddUserToChannelAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var request = new AddUserToChannelRequest
        {
            TargetUserId = Ulid.NewUlid().ToString(),
            ChannelPermissionType = ChannelPermissionType.Owner
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channelId}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName =
        "POST /channels/{channelId}/users: AddUserToChannelAsync는 만약 허용되지 않은 사용자가 요청한 경우 403 Forbidden을 반환합니다.")]
    public async Task Is_AddUserToChannelAsync_Returns_403_For_User_Without_Proper_Permission()
    {
        // Let
        var (user, token) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var (secondUser, secondToken) = await CreateAccountAsync();
        var request = new AddUserToChannelRequest
        {
            TargetUserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };

        // Do
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondToken.Token);
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName =
        "POST /channels/{channelId}/users: AddUserToChannelAsync는 만약 이미 채널에 사용자가 있는 경우 409 Conflict를 반환합니다.")]
    public async Task Is_AddUserToChannelAsync_Returns_409_Conflict_When_User_Already_Exists()
    {
        // Let
        var databaseContext = GetRequiredService<DatabaseContext>();
        var (user, token) = await CreateAccountAsync();
        var (secondUser, secondToken) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var permission = new ChannelPermission
        {
            ChannelId = channel.Id,
            UserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Reader
        };
        databaseContext.ChannelPermissions.Add(permission);
        await databaseContext.SaveChangesAsync();
        var request = new AddUserToChannelRequest
        {
            TargetUserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact(DisplayName = "POST /channels/{channelId}/users: AddUserToChannelAsync는 채널 권한 정보를 추가합니다.")]
    public async Task Is_AddUserToChannelAsync_Adds_Channel_Permission_Well()
    {
        // Let
        var databaseContext = GetRequiredService<DatabaseContext>();
        var (user, token) = await CreateAccountAsync();
        var (secondUser, secondToken) = await CreateAccountAsync();
        var channel = await CreateChannelAsync(token.Token);
        var request = new AddUserToChannelRequest
        {
            TargetUserId = secondUser.Id,
            ChannelPermissionType = ChannelPermissionType.Owner
        };
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        // Do
        var response = await WebRequestClient.PostAsJsonAsync($"/channels/{channel.Id}/users", request);

        // Check
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check Data
        var addedChannelPermission = await databaseContext.ChannelPermissions
                                                          .AsNoTracking()
                                                          .Where(a => a.ChannelId == channel.Id && a.UserId == secondUser.Id)
                                                          .SingleAsync();
        Assert.Equal(request.TargetUserId, addedChannelPermission.UserId);
        Assert.Equal(request.ChannelPermissionType, addedChannelPermission.ChannelPermissionType);
    }
}