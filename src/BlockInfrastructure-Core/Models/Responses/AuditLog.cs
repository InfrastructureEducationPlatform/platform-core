namespace BlockInfrastructure.Core.Models.Responses;

public class AuditLog
{
    public string AuditName { get; set; }
    public DateTimeOffset AuditTime { get; set; }
}