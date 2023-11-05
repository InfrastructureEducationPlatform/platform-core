using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure_Core.Models.Data;

public class User : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string? ProfilePictureImageUrl { get; set; }

    public List<Credential> CredentialList { get; set; }
}