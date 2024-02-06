using System.Net;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Service;

public class ChannelServiceTest
{
    private readonly ChannelService _channelService;

    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    public ChannelServiceTest()
    {
        _channelService = new ChannelService(_databaseContext);
    }

    [Fact(DisplayName = "CreateChannelAsync: CreateChannelAsync는 사용자 요청에 따라 채널을 생성하고, 사용자 본인을 채널 권한에 추가합니다.")]
    public async Task Is_CreateChannelAsync_Creates_Channel_Adds_User_To_Permission_Well()
    {
        // Let 
        var request = new CreateChannelRequest
        {
            Name = "TestChannel",
            Description = "TestDescription",
            ImageUrl = null
        };
        var contextUser = new ContextUser
        {
            UserId = Ulid.NewUlid().ToString(),
            Email = "KangDroid"
        };

        // Do
        await _channelService.CreateChannelAsync(request, contextUser);

        // Check
        var channel = await _databaseContext.Channels
                                            .Include(a => a.ChannelPermissionList)
                                            .SingleAsync();
        Assert.Equal(request.Name, channel.Name);
        Assert.Equal(request.Description, channel.Description);
        Assert.Equal(request.ImageUrl, channel.ProfileImageUrl);
        Assert.Single(channel.ChannelPermissionList);
        Assert.Equal(contextUser.UserId, channel.ChannelPermissionList.First().UserId);
        Assert.Equal(ChannelPermissionType.Owner, channel.ChannelPermissionList.First().ChannelPermissionType);
    }

    [Fact(DisplayName =
        "GetChannelInformationAsync: GetChannelInformationAsync는 만약 채널 정보가 존재하지 않는 경우, ApiException에 ChannelError.ChannelNotFound를 던집니다.")]
    public async Task Is_GetChannelInformationAsync_Throws_Exception_When_Channel_Not_Found()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();

        // Do
        var exception =
            await Assert.ThrowsAsync<ApiException>(async () => await _channelService.GetChannelInformationAsync(channelId));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(ChannelError.ChannelNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "GetChannelInformationAsync: GetChannelInformationAsync는 채널 정보를 가져옵니다.")]
    public async Task Is_GetChannelInformationAsync_Returns_Channel_Information_Well()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "TestChannel",
            Description = "TestDescription",
            ProfileImageUrl = null,
            ChannelPermissionList = new List<ChannelPermission>
            {
                new()
                {
                    ChannelPermissionType = ChannelPermissionType.Owner,
                    User = new User
                    {
                        Id = Ulid.NewUlid().ToString(),
                        Name = "KangDroid",
                        Email = "kangdroid@test.com",
                        ProfilePictureImageUrl = null
                    }
                }
            }
        };
        _databaseContext.Channels.Add(channel);
        await _databaseContext.SaveChangesAsync();

        // Do
        var response = await _channelService.GetChannelInformationAsync(channel.Id);

        // Check
        Assert.Equal(channel.Id, response.ChannelId);
        Assert.Equal(channel.Name, response.Name);
        Assert.Equal(channel.Description, response.Description);
        Assert.Equal(channel.ProfileImageUrl, response.ProfileImageUrl);
        Assert.Single(response.ChannelUserInformationList);
        Assert.Equal(channel.ChannelPermissionList.First().UserId, response.ChannelUserInformationList.First().UserId);
        Assert.Equal(channel.ChannelPermissionList.First().User.Name, response.ChannelUserInformationList.First().Name);
        Assert.Equal(channel.ChannelPermissionList.First().User.Email, response.ChannelUserInformationList.First().Email);
        Assert.Equal(channel.ChannelPermissionList.First().User.ProfilePictureImageUrl,
            response.ChannelUserInformationList.First().ProfilePictureImageUrl);
    }

    [Fact(DisplayName =
        "UpdateChannelInformationAsync: UpdateChannelInformationAsync는 만약 채널 정보가 존재하지 않는 경우, ApiException에 ChannelError.ChannelNotFound를 던집니다.")]
    public async Task Is_UpdateChannelInformationAsync_Throws_Exception_When_Channel_Not_Found()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var request = new UpdateChannelInformationRequest
        {
            ChannelName = "TestChannel",
            ChannelDescription = "TestDescription",
            ProfileImageUrl = null
        };

        // Do
        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            await _channelService.UpdateChannelInformationAsync(channelId, request));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(ChannelError.ChannelNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "UpdateChannelInformationAsync: UpdateChannelInformationAsync는 채널 정보를 수정합니다.")]
    public async Task Is_UpdateChannelInformationAsync_Updates_Channel_Information_Well()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "TestChannel",
            Description = "TestDescription",
            ProfileImageUrl = null
        };
        _databaseContext.Channels.Add(channel);
        await _databaseContext.SaveChangesAsync();

        var request = new UpdateChannelInformationRequest
        {
            ChannelName = "UpdatedChannel",
            ChannelDescription = "UpdatedDescription",
            ProfileImageUrl = "UpdatedImageUrl"
        };

        // Do
        await _channelService.UpdateChannelInformationAsync(channel.Id, request);

        // Check
        var updatedChannel = await _databaseContext.Channels.SingleAsync();
        Assert.Equal(request.ChannelName, updatedChannel.Name);
        Assert.Equal(request.ChannelDescription, updatedChannel.Description);
        Assert.Equal(request.ProfileImageUrl, updatedChannel.ProfileImageUrl);
    }
}