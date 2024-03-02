using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class DeploymentProjection
{
    [Required]
    public string DeploymentId { get; set; }

    [Required]
    public string SketchId { get; set; }

    [Required]
    public string PluginId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DeploymentStatus DeploymentStatus { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    public JsonDocument? DeploymentOutput { get; set; }

    public static DeploymentProjection FromDeploymentLog(DeploymentLog deploymentLog)
    {
        return new DeploymentProjection
        {
            DeploymentId = deploymentLog.Id,
            SketchId = deploymentLog.SketchId,
            PluginId = deploymentLog.PluginInstallation.PluginId,
            DeploymentStatus = deploymentLog.DeploymentStatus,
            DeploymentOutput = deploymentLog.DeploymentOutput,
            CreatedAt = deploymentLog.CreatedAt
        };
    }
}