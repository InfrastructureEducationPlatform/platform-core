using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Data;

public class DeploymentLog : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string SketchId { get; set; }
    public Sketch Sketch { get; set; }

    public string PluginId { get; set; }
    public Plugin Plugin { get; set; }

    public DeploymentStatus DeploymentStatus { get; set; }

    public JsonDocument? DeploymentOutput { get; set; }
}