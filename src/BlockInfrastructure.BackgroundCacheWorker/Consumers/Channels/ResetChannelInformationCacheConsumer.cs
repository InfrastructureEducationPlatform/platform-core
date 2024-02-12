using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BlockInfrastructure.BackgroundCacheWorker.Consumers.Channels;

public class ResetChannelInformationCacheConsumer(
    ILogger<ResetChannelInformationCacheConsumer> logger,
    ICacheService cacheService) : IConsumer<ChannelStateModifiedEvent>
{
    public async Task Consume(ConsumeContext<ChannelStateModifiedEvent> context)
    {
        logger.LogInformation("Invalidating Cache for channel.modified {ChannelId}", context.Message.ChannelId);
        await cacheService.DeleteAsync(CacheKeys.ChannelInformationKey(context.Message.ChannelId));
    }
}