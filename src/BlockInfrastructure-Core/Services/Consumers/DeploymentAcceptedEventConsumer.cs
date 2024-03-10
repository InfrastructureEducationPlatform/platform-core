using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services.Consumers;

public class DeploymentAcceptedEventConsumer(ILogger<DeploymentAcceptedEventConsumer> logger, DatabaseContext databaseContext)
    : IConsumer<DeploymentAcceptedEvent>
{
    public async Task Consume(ConsumeContext<DeploymentAcceptedEvent> context)
    {
        var deploymentId = context.Message.DeploymentId;
        logger.LogInformation("Deployment accepted: {DeploymentId}", deploymentId);

        // Find Deployment Log
        var deploymentLog = await databaseContext.DeploymentLogs.SingleAsync(x => x.Id == deploymentId);
        deploymentLog.DeploymentStatus = DeploymentStatus.Deploying;
        await databaseContext.SaveChangesAsync();
    }
}