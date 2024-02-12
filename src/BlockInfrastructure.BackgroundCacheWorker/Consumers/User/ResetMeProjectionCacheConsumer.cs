using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BlockInfrastructure.BackgroundCacheWorker.Consumers.User;

public class ResetMeProjectionCacheConsumer(
    ILogger<ResetMeProjectionCacheConsumer> logger,
    ICacheService cacheService) : IConsumer<UserStateModifiedEvent>
{
    public async Task Consume(ConsumeContext<UserStateModifiedEvent> context)
    {
        logger.LogInformation("Invalidating Cache for user.modified {UserId}", context.Message.UserId);
        await cacheService.DeleteAsync(CacheKeys.UserMeProjectionKey(context.Message.UserId));
    }
}