using System.Net;
using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlockInfrastructure.Core.Test.Service;

public class UserServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly Mock<ICacheService> _mockCacheService = new();

    private readonly IUserService _userService;

    public UserServiceTest()
    {
        _userService = new UserService(_databaseContext, _mockCacheService.Object);
    }

    [Fact(DisplayName = "GetMeAsync: GetMeAsync는 만약 사용자를 찾을 수 없는 경우 ApiException을 던집니다.")]
    public async Task Is_GetMeAsync_Throws_ApiException_When_User_Not_Found()
    {
        // Let
        var contextUser = new ContextUser
        {
            UserId = Ulid.NewUlid().ToString(),
            Email = Ulid.NewUlid().ToString()
        };
        _mockCacheService.Setup(a =>
                             a.GetOrSetAsync(CacheKeys.UserMeProjectionKey(contextUser.UserId),
                                 It.IsAny<Func<Task<MeProjection>>>(), It.IsAny<TimeSpan>()))
                         .ThrowsAsync(new ApiException(HttpStatusCode.NotFound,
                             "Unknown error: Cannot find user!", UserError.UserNotFound));

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() => _userService.GetMeAsync(contextUser));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(UserError.UserNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "GetMeAsync: GetMeAsync는 사용자 정보가 정상적으로 있는 경우 채널 권한과 함께 MeResponse를 반환합니다.")]
    public async Task Is_GetMeAsync_Returns_MeResponse_With_Channel_Permission_Well()
    {
        // Let
        var user = new User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null,
            ChannelPermissionList = new List<ChannelPermission>
            {
                new()
                {
                    ChannelId = "test",
                    Channel = new Channel
                    {
                        Name = "Test",
                        Id = "test",
                        Description = ""
                    }
                }
            }
        };
        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync();
        var contextUser = new ContextUser
        {
            UserId = user.Id,
            Email = user.Email
        };
        _mockCacheService.Setup(a =>
                             a.GetOrSetAsync(CacheKeys.UserMeProjectionKey(contextUser.UserId),
                                 It.IsAny<Func<Task<MeProjection>>>(), It.IsAny<TimeSpan>()))
                         .ReturnsAsync(MeProjection.FromUser(user));

        // Do
        var meResponse = await _userService.GetMeAsync(contextUser);

        // Check
        Assert.Equal(user.Id, meResponse.UserId);
        Assert.Equal(user.Email, meResponse.Email);
        Assert.Equal(user.Name, meResponse.Name);
        Assert.Equal(user.ProfilePictureImageUrl, meResponse.ProfilePictureImageUrl);
        Assert.Single(meResponse.ChannelPermissionList);
    }

    [Fact(DisplayName = "UpdatePreferenceAsync: UpdatePreferenceAsync는 만약 사용자를 찾을 수 없는 경우 ApiException을 던집니다.")]
    public async Task Is_UpdatePreferenceAsync_Throws_ApiException_When_User_Not_Found()
    {
        // Let
        var contextUser = new ContextUser
        {
            UserId = Ulid.NewUlid().ToString(),
            Email = Ulid.NewUlid().ToString()
        };
        var updateUserPreferenceRequest = new UpdateUserPreferenceRequest
        {
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null
        };
        _databaseContext.Users.Add(new User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null
        });
        await _databaseContext.SaveChangesAsync();

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() =>
            _userService.UpdatePreferenceAsync(contextUser, updateUserPreferenceRequest));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(UserError.UserNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "UpdatePreferenceAsync: UpdatePreferenceAsync는 사용자 정보가 정상적으로 있는 경우 사용자 정보를 업데이트합니다.")]
    public async Task Is_UpdatePreferenceAsync_Updates_User_When_User_Found()
    {
        // Let
        var contextUser = new ContextUser
        {
            UserId = Ulid.NewUlid().ToString(),
            Email = Ulid.NewUlid().ToString()
        };
        var updateUserPreferenceRequest = new UpdateUserPreferenceRequest
        {
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null
        };
        var user = new User
        {
            Id = contextUser.UserId,
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null
        };
        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync();

        // Do
        await _userService.UpdatePreferenceAsync(contextUser, updateUserPreferenceRequest);

        // Check
        Assert.Equal(updateUserPreferenceRequest.Name, user.Name);
        Assert.Equal(updateUserPreferenceRequest.Email, user.Email);
        Assert.Equal(updateUserPreferenceRequest.ProfilePictureImageUrl, user.ProfilePictureImageUrl);
    }

    [Fact(DisplayName = "SearchUserAsync: SearchUserAsync는 사용자를 정상적으로 찾아옵니다.")]
    public async Task Is_SearchUserAsync_Returns_User_When_User_Found()
    {
        // Let
        var user = new User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null
        };
        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync();

        // Do
        var searchResponse = await _userService.SearchUserAsync("KangDroid");

        // Check
        Assert.Single(searchResponse);
        Assert.Equal(user.Id, searchResponse.First().UserId);
        Assert.Equal(user.Email, searchResponse.First().UserEmail);
        Assert.Equal(user.Name, searchResponse.First().UserName);
        Assert.Equal(user.ProfilePictureImageUrl, searchResponse.First().ProfilePictureImageUrl);
    }

    [Fact(DisplayName = "DeleteUserAsync: DeleteUserAsync는 만약 사용자를 찾을 수 없는 경우 ApiException을 던집니다.")]
    public async Task Is_DeleteUserAsync_Throws_ApiException_When_User_Not_Found()
    {
        // Let
        var userId = Ulid.NewUlid().ToString();

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() => _userService.DeleteUserAsync(userId));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(UserError.UserNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "DeleteUserAsync: DeleteUserAsync는 사용자를 정상적으로 삭제합니다.")]
    public async Task Is_DeleteUserAsync_Deletes_User_When_User_Found()
    {
        // Let
        var user = new User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null
        };
        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync();

        // Do
        await _userService.DeleteUserAsync(user.Id);

        // Check
        Assert.Empty(_databaseContext.Users);
    }
}