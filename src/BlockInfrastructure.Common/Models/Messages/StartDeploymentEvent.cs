using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Common.Models.Messages;

public class StartDeploymentEvent
{
    public string DeploymentLogId { get; set; }

    public SketchBlockProjection SketchProjection { get; set; }

    public PluginInstallationProjection PluginInstallationProjection { get; set; }

    public static StartDeploymentEvent FromDeploymentLog(DeploymentLog deploymentLog)
    {
        return new StartDeploymentEvent
        {
            DeploymentLogId = deploymentLog.Id,
            SketchProjection = SketchBlockProjection.FromSketch(deploymentLog.Sketch, deploymentLog.CapturedBlockData),
            PluginInstallationProjection =
                PluginInstallationProjection.FromPluginInstallation(deploymentLog.PluginInstallation)
        };
    }
}

public class SketchBlockProjection
{
    public string SketchId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public JsonDocument BlockSketch { get; set; }

    public static SketchBlockProjection FromSketch(Sketch sketch, JsonDocument? blockSketchOverride = null)
    {
        return new SketchBlockProjection
        {
            SketchId = sketch.Id,
            Name = sketch.Name,
            Description = sketch.Description,
            BlockSketch = blockSketchOverride ?? sketch.BlockSketch
        };
    }
}

public class PluginInstallationProjection
{
    public string PluginInstallationId { get; set; }
    public string PluginId { get; set; }
    public JsonDocument PluginConfiguration { get; set; }

    public static PluginInstallationProjection FromPluginInstallation(PluginInstallation pluginInstallation)
    {
        return new PluginInstallationProjection
        {
            PluginInstallationId = pluginInstallation.Id,
            PluginId = pluginInstallation.PluginId,
            PluginConfiguration = pluginInstallation.PluginConfiguration
        };
    }
}