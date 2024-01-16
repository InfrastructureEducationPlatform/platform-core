using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure.Common.Models.Data;

public class RefreshToken : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}