using System.Net;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public interface IDeploymentService
{
    Task<DeploymentLog> GetDeploymentAsync(string deploymentId);
}

public class DeploymentService(DatabaseContext databaseContext) : IDeploymentService
{
    public async Task<DeploymentLog> GetDeploymentAsync(string deploymentId)
    {
        return await databaseContext.DeploymentLogs
                                    .FirstOrDefaultAsync(x => x.Id == deploymentId) ??
               throw new ApiException(HttpStatusCode.NotFound, $"Cannot find Deployment ID {deploymentId}!",
                   DeploymentError.DeploymentNotFound);
    }
}