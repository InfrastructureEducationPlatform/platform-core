using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockInfrastructure.BackgroundCacheWorker.Consumers.User;

public class ResetMeProjectionCacheConsumer(
    ILogger<ResetMeProjectionCacheConsumer> logger,
    ICacheService cacheService,
    DatabaseContext databaseContext) : IConsumer<UserStateModifiedEvent>
{
    public async Task Consume(ConsumeContext<UserStateModifiedEvent> context)
    {
        logger.LogInformation("Invalidating Cache for user.modified {UserId}", context.Message.UserId);

        // Data
        var user = (await databaseContext.Users
                                         .Include(a => a.ChannelPermissionList)
                                         .ThenInclude(a => a.Channel)
                                         .Where(a => a.Id == context.Message.UserId)
                                         .SingleOrDefaultAsync())!;
        // Set Cache
        await cacheService.SetAsync(CacheKeys.UserMeProjectionKey(context.Message.UserId), MeProjection.FromUser(user),
            TimeSpan.FromDays(10));
    }
}