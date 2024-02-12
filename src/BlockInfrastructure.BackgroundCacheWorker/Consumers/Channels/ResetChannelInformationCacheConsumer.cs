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
        switch (context.Message.ChannelStateModifiedEventType)
        {
            case ChannelStateModifiedEventType.ForChannel:
                var channelMessage = ChannelStateModifiedForChannel.FromChannelStateModifiedEvent(context.Message);
                await DeleteCacheForChannelAsync(channelMessage.ChannelId);
                break;
            case ChannelStateModifiedEventType.ForUser:
                var userMessage = ChannelStateModifiedForUser.FromChannelStateModifiedEvent(context.Message);
                await DeleteCacheForUserAsync(userMessage.UserId);
                break;
        }
    }

    private async Task DeleteCacheForChannelAsync(string channelId)
    {
        logger.LogInformation($"Invalidating cache from key {CacheKeys.ChannelInformationKey(channelId)}");
        await cacheService.DeleteAsync(CacheKeys.ChannelInformationKey(channelId));
    }

    private async Task DeleteCacheForUserAsync(string userId)
    {
        // Get Channels for user
        var channelPermissionList = await databaseContext.ChannelPermissions
                                                         .Where(a => a.UserId == userId)
                                                         .ToListAsync();
        foreach (var eachPermission in channelPermissionList)
        {
            await DeleteCacheForChannelAsync(eachPermission.ChannelId);
        }
    }
}