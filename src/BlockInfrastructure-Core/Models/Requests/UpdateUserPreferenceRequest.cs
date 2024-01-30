using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure.Core.Models.Requests;

public class UpdateUserPreferenceRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public string? ProfilePictureImageUrl { get; set; }
}