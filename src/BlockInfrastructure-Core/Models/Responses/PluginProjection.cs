using System.ComponentModel.DataAnnotations;
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
    public List<PluginTypeDefinition> PluginTypeDefinitions { get; set; } = new();


    public static PluginProjection FromPlugin(Plugin plugin)
    {
        return new PluginProjection
        {
            Id = plugin.Id,
            Name = plugin.Name,
            Description = plugin.Description,
            PluginTypeDefinitions = plugin.PluginTypeDefinitions
        };
    }
}