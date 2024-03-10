using System.Net;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public interface IDeploymentService
{
    Task<DeploymentLog> GetDeploymentAsync(string deploymentId);
    Task<List<DeploymentLog>> ListDeploymentForChannelAsync(IEnumerable<string> includedChannelId);
    Task DestroyDeploymentAsync(string deploymentId);
}

public class DeploymentService(DatabaseContext databaseContext, ISendEndpointProvider sendEndpointProvider)
    : IDeploymentService
{
    public async Task<DeploymentLog> GetDeploymentAsync(string deploymentId)
    {
        return await databaseContext.DeploymentLogs
                                    .Include(a => a.Channel)
                                    .Include(a => a.Sketch)
                                    .Include(a => a.PluginInstallation)
                                    .FirstOrDefaultAsync(x => x.Id == deploymentId) ??
               throw new ApiException(HttpStatusCode.NotFound, $"Cannot find Deployment ID {deploymentId}!",
                   DeploymentError.DeploymentNotFound);
    }

    public async Task<List<DeploymentLog>> ListDeploymentForChannelAsync(IEnumerable<string> includedChannelId)
    {
        return await databaseContext.DeploymentLogs
                                    .Include(a => a.Channel)
                                    .Include(a => a.Sketch)
                                    .Include(a => a.PluginInstallation)
                                    .Where(x => includedChannelId.Contains(x.ChannelId))
                                    .ToListAsync();
    }

    public async Task DestroyDeploymentAsync(string deploymentId)
    {
        var latestDeployment = await databaseContext.DeploymentLogs
                                                    .Include(a => a.Sketch)
                                                    .Include(a => a.PluginInstallation)
                                                    .OrderByDescending(a => a.Id)
                                                    .FirstOrDefaultAsync() ?? throw new ApiException(HttpStatusCode.NotFound,
            $"Cannot find Deployment ID {deploymentId}!", DeploymentError.DeploymentNotFound);

        if (latestDeployment.Id != deploymentId)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "Cannot delete non-latest deployment!",
                DeploymentError.CannotDeleteNonLatestDeployment);
        }

        // Send Message
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:deployment.destroy"));
        await sendEndpoint.Send(StartDeploymentEvent.FromDeploymentLog(latestDeployment));
    }
}