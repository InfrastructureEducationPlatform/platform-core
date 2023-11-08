using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Data;

public class RequestLog : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    public string? UserId { get; set; }

    public string Scheme { get; set; }

    public string HttpMethod { get; set; }

    public string Path { get; set; }

    public string? QueryString { get; set; }

    public int StatusCode { get; set; }
    public JsonDocument? RequestHeaders { get; set; }
    public JsonDocument? ResponseHeaders { get; set; }

    public JsonDocument? RequestBody { get; set; }
    public JsonDocument? ResponseBody { get; set; }
}