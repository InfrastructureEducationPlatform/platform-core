using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Core.Models.Requests;

public class CreateSketchRequest
{
    /// <summary>
    ///     스케치 이름입니다.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    ///     스케치 설명입니다.
    /// </summary>
    [Required]
    public string Description { get; set; }

    [Required]
    public JsonDocument BlockSketch { get; set; }
}