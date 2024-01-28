using System.Net;
using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public interface IUserService
{
    Task<MeProjection> GetMeAsync(ContextUser contextUser);
}

public class UserService(DatabaseContext databaseContext, ICacheService cacheService) : IUserService
{
    public async Task<MeProjection> GetMeAsync(ContextUser contextUser)
    {
        var data = await cacheService.GetOrSetAsync(CacheKeys.UserMeProjectionKey(contextUser.UserId), async () =>
        {
            // Get User
            var user = await databaseContext.Users
                                            .Include(a => a.ChannelPermissionList)
                                            .ThenInclude(a => a.Channel)
                                            .Where(a => a.Id == contextUser.UserId)
                                            .SingleOrDefaultAsync() ?? throw new ApiException(HttpStatusCode.NotFound,
                "Unknown error: Cannot find user!", UserError.UserNotFound);
            return MeProjection.FromUser(user);
        }, TimeSpan.FromDays(10));

        return data!;
    }
}