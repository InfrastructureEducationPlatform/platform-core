using System.Net;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public interface IDeploymentService
{
    Task<DeploymentLog> GetDeploymentAsync(string deploymentId);
    Task<List<DeploymentLog>> ListDeploymentForChannelAsync(IEnumerable<string> includedChannelId);
}

public class DeploymentService(DatabaseContext databaseContext) : IDeploymentService
{
    public async Task<DeploymentLog> GetDeploymentAsync(string deploymentId)
    {
        return await databaseContext.DeploymentLogs
                                    .Include(a => a.PluginInstallation)
                                    .FirstOrDefaultAsync(x => x.Id == deploymentId) ??
               throw new ApiException(HttpStatusCode.NotFound, $"Cannot find Deployment ID {deploymentId}!",
                   DeploymentError.DeploymentNotFound);
    }

    public async Task<List<DeploymentLog>> ListDeploymentForChannelAsync(IEnumerable<string> includedChannelId)
    {
        return await databaseContext.DeploymentLogs
                                    .Include(a => a.PluginInstallation)
                                    .Where(x => includedChannelId.Contains(x.ChannelId))
                                    .ToListAsync();
    }
}