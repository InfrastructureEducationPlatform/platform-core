using System.Net.Http.Headers;
using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlockInfrastructure.Common.Services;

public class AuthenticationMiddleware(RequestDelegate next, IJwtService jwtService)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Resolve Header
        var headerParseResult = AuthenticationHeaderValue.TryParse(httpContext.Request.Headers.Authorization, out var header);

        if (headerParseResult && header?.Parameter != null)
        {
            // Validate JWT
            var jwtValidationResult = jwtService.ValidateJwt(header.Parameter);
            if (jwtValidationResult != null)
            {
                // Set Context User
                var contextUser = new ContextUser
                {
                    Email = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Email).Value,
                    UserId = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Sub).Value
                };
                httpContext.SetUserContext(contextUser);
            }
        }

        await next(httpContext);
    }
}