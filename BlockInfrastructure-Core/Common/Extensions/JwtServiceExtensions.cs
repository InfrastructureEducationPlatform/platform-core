using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Services;

namespace BlockInfrastructure.Core.Common.Extensions;

public static class JwtServiceExtensions
{
    public static string GenerateJwtForUser(this IJwtService jwtService, User user)
    {
        var claimList = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new("name", user.Name),
            new("profileUrl", user.ProfilePictureImageUrl ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        return jwtService.GenerateJwt(claimList, DateTime.Now.AddHours(1));
    }

    public static string GenerateJwtForRegistration(this IJwtService jwtService, CredentialProvider provider,
                                                    OAuthResult oAuthResult)
    {
        var claimList = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, oAuthResult.OAuthId),
            new(JwtRegisteredClaimNames.Email, oAuthResult.Email),
            new("provider", provider.ToString())
        };

        return jwtService.GenerateJwt(claimList, DateTime.Now.AddMinutes(10));
    }

    public static (OAuthResult, CredentialProvider) ValidateJwtForRegistration(this IJwtService jwtService, string joinToken)
    {
        var jwtToken = jwtService.ValidateJwt(joinToken) ?? throw new ApiException(HttpStatusCode.BadRequest,
            "Join Token validation failed!",
            AuthError.JoinTokenValidationFailed);

        var provider = jwtToken.Claims.FirstOrDefault(a => a.Type == "provider")?.Value;
        var oAuthId = jwtToken.Claims.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var email = jwtToken.Claims.FirstOrDefault(a => a.Type == JwtRegisteredClaimNames.Email)?.Value;

        return (new OAuthResult
        {
            Email = email,
            OAuthId = oAuthId
        }, Enum.Parse<CredentialProvider>(provider));
    }
}