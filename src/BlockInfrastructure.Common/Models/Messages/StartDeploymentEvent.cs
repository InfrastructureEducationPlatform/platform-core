using BlockInfrastructure.Common.Models.Data;
using MassTransit;

namespace BlockInfrastructure.Common.Models.Messages;

[EntityName("deployment.started")]
public class StartDeploymentEvent
{
    /// <summary>
    ///     Deployment Log - 스케치와 플러그인이 포함되어 있음.
    /// </summary>
    public DeploymentLog DeploymentLog { get; set; }
}