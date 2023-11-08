using BlockInfrastructure.Core.Models.Data;

namespace BlockInfrastructure.Core.Models.Internal;

public class ContextUser
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public CredentialProvider CredentialProvider { get; set; }
}