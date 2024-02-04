using BlockInfrastructure.Common.Models.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BlockInfrastructure.Common.Services;

public class InvalidateCacheInterceptor : SaveChangesInterceptor
{
    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
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

            if (eachEntry.State == EntityState.Added || eachEntry.State == EntityState.Deleted ||
                eachEntry.State == EntityState.Modified)
            {
                // Send Cache Invalidation Message
                foreach (var eachMessageObject in support.GetCacheEventMessage())
                {
                    await massTransit.Publish(eachMessageObject, cancellationToken);
                }
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}