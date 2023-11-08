using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlockInfrastructure.Core.Common;

[ExcludeFromCodeCoverage]
public class JwtAuthenticationFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var jwtService = httpContext.RequestServices.GetRequiredService<IJwtService>();

        // Resolve Header
        var headerParseResult = AuthenticationHeaderValue.TryParse(httpContext.Request.Headers.Authorization, out var header);

        if (!headerParseResult || header?.Parameter == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Validate JWT
        var jwtValidationResult = jwtService.ValidateJwt(header.Parameter);
        if (jwtValidationResult == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Set Context User
        var contextUser = new ContextUser
        {
            CredentialProvider =
                Enum.Parse<CredentialProvider>(jwtValidationResult.Claims.First(a => a.Type == "provider").Value),
            Email = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Email).Value,
            UserId = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Sub).Value
        };
        httpContext.SetUserContext(contextUser);

        await next();
    }
}