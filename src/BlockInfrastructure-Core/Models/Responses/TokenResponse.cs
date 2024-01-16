using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlockInfrastructure.Core.Models.Responses;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoginResult
{
    NeedsRegistration,
    LoginSucceed
}

public class TokenResponse
{
    /// <summary>
    ///     로그인 성공 여부 - 성공인 경우 Token = AccessToken, RefreshToken = !null, 실패인 경우 Token = JoinToken, RefreshToken = null
    /// </summary>
    [Required]
    public LoginResult LoginResult { get; set; }

    /// <summary>
    ///     Access Token, expires in an hour.
    /// </summary>
    [Required]
    public string Token { get; set; }

    /// <summary>
    ///     Refresh Token, expires in 30 days.
    /// </summary>
    public string? RefreshToken { get; set; }
}