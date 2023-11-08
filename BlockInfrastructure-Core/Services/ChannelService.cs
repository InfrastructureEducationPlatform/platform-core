using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Models.Requests;

namespace BlockInfrastructure.Core.Services;

public class ChannelService(DatabaseContext databaseContext)
{
    public async Task CreateChannelAsync(CreateChannelRequest createChannelRequest, ContextUser contextUser)
    {
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = createChannelRequest.Name,
            Description = createChannelRequest.Description,
            ProfileImageUrl = createChannelRequest.ImageUrl,
            ChannelPermissionList = new List<ChannelPermission>
            {
                new()
                {
                    UserId = contextUser.UserId,
                    ChannelPermissionType = ChannelPermissionType.Owner
                }
            }
        };
        databaseContext.Channels.Add(channel);
        await databaseContext.SaveChangesAsync();
    }
}