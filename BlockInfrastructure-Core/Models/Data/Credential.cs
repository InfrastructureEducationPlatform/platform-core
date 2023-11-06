namespace BlockInfrastructure.Core.Models.Data;

public class Credential : AutomaticAuditSupport
{
    public string CredentialId { get; set; }
    public CredentialProvider CredentialProvider { get; set; }
    public string? CredentialKey { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}