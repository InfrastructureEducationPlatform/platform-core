using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure.Core.Models.Requests;

public class RefreshTokenRequest
{
    /// <summary>
    ///     만료된 엑세스 토큰
    /// </summary>
    [Required]
    public string AccessToken { get; set; }

    /// <summary>
    ///     리프레시 토큰
    /// </summary>
    [Required]
    public string RefreshToken { get; set; }
}