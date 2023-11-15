using System.Net;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Test.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Service;

public class UserServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userService = new UserService(_databaseContext);
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

        // Do
        var meResponse = await _userService.GetMeAsync(contextUser);

        // Check
        Assert.Equal(user.Id, meResponse.UserId);
        Assert.Equal(user.Email, meResponse.Email);
        Assert.Equal(user.Name, meResponse.Name);
        Assert.Equal(user.ProfilePictureImageUrl, meResponse.ProfilePictureImageUrl);
        Assert.Single(meResponse.ChannelPermissionList);
    }
}