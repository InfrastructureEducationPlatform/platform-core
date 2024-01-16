using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure.Core.Models.Requests;

public class CreateChannelRequest
{
    /// <summary>
    ///     생성할 채널의 이름입니다.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    ///     생성할 채널의 1줄 설명입니다.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    ///     Optional: 채널의 프로필 이미지 URL입니다.
    /// </summary>
    public string? ImageUrl { get; set; }
}