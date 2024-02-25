using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class PluginProjection
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public JsonDocument SamplePluginConfiguration { get; set; }

    [Required]
    public List<PluginTypeDefinition> PluginTypeDefinitions { get; set; } = new();


    public static PluginProjection FromPlugin(Plugin plugin)
    {
        return new PluginProjection
        {
            Id = plugin.Id,
            Name = plugin.Name,
            Description = plugin.Description,
            SamplePluginConfiguration = plugin.SamplePluginConfiguration,
            PluginTypeDefinitions = plugin.PluginTypeDefinitions
        };
    }

    public static PluginProjection FromPluginInstallation(PluginInstallation pluginInstallation)
    {
        return new PluginProjection
        {
            Id = pluginInstallation.PluginId,
            Name = pluginInstallation.Plugin.Name,
            Description = pluginInstallation.Plugin.Description,
            SamplePluginConfiguration = pluginInstallation.Plugin.SamplePluginConfiguration,
            PluginTypeDefinitions = pluginInstallation.Plugin.PluginTypeDefinitions
        };
    }
}