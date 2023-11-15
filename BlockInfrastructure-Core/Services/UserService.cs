using System.Net;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class UserService(DatabaseContext databaseContext)
{
    public async Task<MeResponse> GetMeAsync(ContextUser contextUser)
    {
        // Get User
        var user = await databaseContext.Users
                                        .Include(a => a.ChannelPermissionList)
                                        .ThenInclude(a => a.Channel)
                                        .Where(a => a.Id == contextUser.UserId)
                                        .SingleOrDefaultAsync() ?? throw new ApiException(HttpStatusCode.NotFound,
            "Unknown error: Cannot find user!", UserError.UserNotFound);

        return new MeResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            ProfilePictureImageUrl = user.ProfilePictureImageUrl,
            ChannelPermissionList = user.ChannelPermissionList
                                        .Select(a => new ChannelPermissionProjection
                                        {
                                            UserId = a.UserId,
                                            ChannelId = a.ChannelId,
                                            ChannelName = a.Channel.Name,
                                            ChannelPermissionType = a.ChannelPermissionType,
                                            CreatedAt = a.CreatedAt
                                        })
                                        .ToList()
        };
    }
}