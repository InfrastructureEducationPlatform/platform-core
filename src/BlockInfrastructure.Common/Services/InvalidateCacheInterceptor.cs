using BlockInfrastructure.Common.Models.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlockInfrastructure.Common.Services;

public class InvalidateCacheInterceptor : SaveChangesInterceptor
{
    public async override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
                                                           CancellationToken cancellationToken = new())
    {
        var massTransit = eventData.Context.GetService<IPublishEndpoint>();
        var entries = eventData.Context.ChangeTracker.Entries();

        foreach (var eachEntry in entries)
        {
            var support = eachEntry.Entity as ICacheEventMessageGenerator;
            if (support == null)
            {
                continue;
            }

            // Send Cache Invalidation Message
            await massTransit.Publish(support.GetCacheEventMessage(), cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}