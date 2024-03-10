using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Common.Models.Data;

public class DeploymentLog : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string SketchId { get; set; }
    public Sketch Sketch { get; set; }

    public string PluginInstallationId { get; set; }
    public PluginInstallation PluginInstallation { get; set; }

    public DeploymentStatus DeploymentStatus { get; set; }

    public JsonDocument? DeploymentOutput { get; set; }

    public Channel Channel { get; set; }
    public string ChannelId { get; set; }

    /// <summary>
    ///     Captured Block Data(JSON Serialized)
    /// </summary>
    public JsonDocument CapturedBlockData { get; set; }
}