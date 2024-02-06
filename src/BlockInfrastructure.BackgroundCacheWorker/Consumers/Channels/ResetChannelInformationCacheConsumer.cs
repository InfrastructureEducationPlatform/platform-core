using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockInfrastructure.BackgroundCacheWorker.Consumers.Channels;

public class ResetChannelInformationCacheConsumer(
    ILogger<ResetChannelInformationCacheConsumer> logger,
    ICacheService cacheService,
    DatabaseContext databaseContext) : IConsumer<ChannelStateModifiedEvent>
{
    public async Task Consume(ConsumeContext<ChannelStateModifiedEvent> context)
    {
        logger.LogInformation("Invalidating Cache for channel.modified {ChannelId}", context.Message.ChannelId);

        // Get Data
        var data = await databaseContext.Channels
                                        .Include(a => a.ChannelPermissionList)
                                        .ThenInclude(a => a.User)
                                        .Where(a => a.Id == context.Message.ChannelId)
                                        .FirstAsync();

        // Set Cache
        await cacheService.SetAsync(CacheKeys.ChannelInformationKey(context.Message.ChannelId), data, TimeSpan.FromDays(10));
    }
}