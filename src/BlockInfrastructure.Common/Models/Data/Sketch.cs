using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Common.Models.Data;

public class Sketch : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public string ChannelId { get; set; }
    public Channel Channel { get; set; }
    public JsonDocument BlockSketch { get; set; }
}