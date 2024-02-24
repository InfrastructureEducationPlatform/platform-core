using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Requests;

public class InstallPluginRequest
{
    [Required]
    public string PluginId { get; set; }

    [Required]
    public JsonDocument PluginConfiguration { get; set; }
}