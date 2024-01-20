using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
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
}