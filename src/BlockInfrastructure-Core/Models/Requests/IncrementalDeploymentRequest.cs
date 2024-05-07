using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Requests;

public class IncrementalDeploymentRequest
{
    [Required]
    public JsonDocument BlockData { get; set; }

    public string PluginId { get; set; }
}