namespace BlockInfrastructure_Core.Models.Data;

public abstract class AutomaticAuditSupport
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}