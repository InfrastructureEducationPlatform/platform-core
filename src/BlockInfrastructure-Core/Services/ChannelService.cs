using System.Net;
using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class ChannelService(DatabaseContext databaseContext, ICacheService cacheService)
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

    public async Task<ChannelInformationResponse> GetChannelInformationAsync(string channelId)
    {
        return await cacheService.GetOrSetAsync(CacheKeys.ChannelInformationKey(channelId), async () =>
        {
            var channelData = await databaseContext.Channels
                                                   .Include(a => a.ChannelPermissionList)
                                                   .ThenInclude(a => a.User)
                                                   .Where(a => a.Id == channelId)
                                                   .FirstOrDefaultAsync() ?? throw new ApiException(HttpStatusCode.NotFound,
                $"채널 정보 ({channelId}를 찾을 수 없습니다!", ChannelError.ChannelNotFound);
            ;
            return ChannelInformationResponse.FromChannelWithUser(channelData);
        }) ?? throw new ApiException(HttpStatusCode.NotFound,
            $"채널 정보 ({channelId}를 찾을 수 없습니다!", ChannelError.ChannelNotFound);
    }

    public async Task UpdateChannelInformationAsync(string channelId,
                                                    UpdateChannelInformationRequest updateChannelInformationRequest)
    {
        var channel = await databaseContext.Channels
                                           .Where(a => a.Id == channelId)
                                           .FirstOrDefaultAsync() ??
                      throw new ApiException(HttpStatusCode.NotFound, $"채널 정보 ({channelId}를 찾을 수 없습니다!",
                          ChannelError.ChannelNotFound);

        channel.Name = updateChannelInformationRequest.ChannelName;
        channel.Description = updateChannelInformationRequest.ChannelDescription;
        channel.ProfileImageUrl = updateChannelInformationRequest.ProfileImageUrl;
        await databaseContext.SaveChangesAsync();
    }

    public async Task UpdateUserChannelRoleAsync(string userId, string channelId,
                                                 UpdateUserChannelRoleRequest updateUserChannelRoleRequest)
    {
        if (userId == updateUserChannelRoleRequest.UserId)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "자신의 권한을 변경할 수 없습니다!",
                ChannelError.CannotChangeOwnRole);
        }

        var channelPermission = await databaseContext.ChannelPermissions
                                                     .Where(a => a.UserId == updateUserChannelRoleRequest.UserId &&
                                                                 a.ChannelId == channelId)
                                                     .FirstOrDefaultAsync() ??
                                throw new ApiException(HttpStatusCode.NotFound,
                                    $"채널 권한 정보 ({updateUserChannelRoleRequest.UserId}를 찾을 수 없습니다!",
                                    ChannelError.ChannelPermissionNotFound);

        channelPermission.ChannelPermissionType = updateUserChannelRoleRequest.ChannelPermissionType;
        await databaseContext.SaveChangesAsync();
    }

    public async Task RemoveUserFromChannelAsync(string requesterUserId, string channelId, string targetUserId)
    {
        if (requesterUserId == targetUserId)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "자신을 채널에서 제거할 수 없습니다!",
                ChannelError.CannotRemoveSelf);
        }

        var channelPermission = await databaseContext.ChannelPermissions
                                                     .Where(a => a.UserId == targetUserId &&
                                                                 a.ChannelId == channelId)
                                                     .FirstOrDefaultAsync() ??
                                throw new ApiException(HttpStatusCode.NotFound,
                                    $"채널 권한 정보 ({targetUserId}를 찾을 수 없습니다!",
                                    ChannelError.ChannelPermissionNotFound);

        databaseContext.ChannelPermissions.Remove(channelPermission);
        await databaseContext.SaveChangesAsync();
    }
}