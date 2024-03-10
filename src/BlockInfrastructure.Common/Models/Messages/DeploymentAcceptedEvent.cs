using MassTransit;

namespace BlockInfrastructure.Common.Models.Messages;

[EntityName("deployment.accepted")]
public class DeploymentAcceptedEvent
{
    public string DeploymentId { get; set; }
}