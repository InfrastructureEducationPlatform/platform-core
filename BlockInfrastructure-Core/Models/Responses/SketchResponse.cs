using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Responses;

public class SketchResponse
{
    public string SketchId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ChannelId { get; set; }
    public JsonDocument BlockSketch { get; set; }
}