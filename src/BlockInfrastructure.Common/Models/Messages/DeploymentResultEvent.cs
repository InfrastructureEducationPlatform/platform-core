using System.Text.Json;
using MassTransit;

namespace BlockInfrastructure.Common.Models.Messages;

[EntityName("deployment.result")]
public class DeploymentResultEvent
{
    public string DeploymentId { get; set; }

    // In List
    public JsonDocument? DeploymentOutputList { get; set; }

    public bool IsSuccess { get; set; }
}