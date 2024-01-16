using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Requests;

public class LoginRequest
{
    /// <summary>
    ///     OAuth 2.0 Provider입니다. String으로 적을 때는 "Google"로 적을 수 있습니다.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public CredentialProvider Provider { get; set; }

    /// <summary>
    ///     OAuth Provider이 보내주는 인가 코드입니다.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string AuthenticationCode { get; set; }
}