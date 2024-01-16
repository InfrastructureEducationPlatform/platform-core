using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Responses;

public class SketchResponse
{
    /// <summary>
    ///     스케치의 Unique ID입니다.
    /// </summary>
    [Required]
    public string SketchId { get; set; }

    /// <summary>
    ///     스케치의 이름입니다.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    ///     스케치의 설명입니다.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    ///     스케치의 채널 ID입니다.
    /// </summary>
    [Required]
    public string ChannelId { get; set; }


    /// <summary>
    ///     스케치의 블록 스케치입니다.
    /// </summary>
    [Required]
    public JsonDocument BlockSketch { get; set; }

    /// <summary>
    ///     생성일
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     마지막 수정일
    /// </summary>
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }
}