using BlockInfrastructure.Core.Models.Data;

namespace BlockInfrastructure.Core.Models.Requests;

public class SignUpRequest
{
    /// <summary>
    ///     User Nickname
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     User Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     Profile Image URL(Can be NULL)
    /// </summary>
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    ///     로그인 API에서 받은 Join Token입니다.
    /// </summary>
    public string JoinToken { get; set; }

    /// <summary>
    ///     Credential Provider(Such as self, google, etc)
    /// </summary>
    public CredentialProvider CredentialProvider { get; set; }
}