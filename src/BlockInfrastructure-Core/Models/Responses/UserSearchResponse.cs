using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class UserSearchResponse
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string UserEmail { get; set; }

    public string? ProfilePictureImageUrl { get; set; }

    public static UserSearchResponse FromUser(User user)
    {
        return new UserSearchResponse
        {
            UserId = user.Id,
            UserEmail = user.Email,
            UserName = user.Name,
            ProfilePictureImageUrl = user.ProfilePictureImageUrl
        };
    }
}