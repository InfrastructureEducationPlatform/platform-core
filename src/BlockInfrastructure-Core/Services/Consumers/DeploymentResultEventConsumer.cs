using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services.Consumers;

public class DeploymentResultEventConsumer(DatabaseContext databaseContext) : IConsumer<DeploymentResultEvent>
{
    public async Task Consume(ConsumeContext<DeploymentResultEvent> context)
    {
        // Find Deployment Log
        var deploymentLog = await databaseContext.DeploymentLogs.SingleAsync(x => x.Id == context.Message.DeploymentId);

        if (context.Message.IsSuccess)
        {
            deploymentLog.DeploymentStatus = DeploymentStatus.Deployed;
            deploymentLog.DeploymentOutput = context.Message.DeploymentOutputList;
        }
        else
        {
            deploymentLog.DeploymentStatus = DeploymentStatus.Failed;
        }

        await databaseContext.SaveChangesAsync();
    }
}