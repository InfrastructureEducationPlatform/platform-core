using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Models.Responses;
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
            context.Result = new ObjectResult(new ErrorResponse
            {
                ErrorMessage = "Cannot authenticate user.",
                ErrorTitle = AuthError.AuthenticationFailed.ErrorTitleToString(),
                StatusCodes = StatusCodes.Status401Unauthorized
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        // Validate JWT
        var jwtValidationResult = jwtService.ValidateJwt(header.Parameter);
        if (jwtValidationResult == null)
        {
            context.Result = new ObjectResult(new ErrorResponse
            {
                ErrorMessage = "Cannot authenticate user.",
                ErrorTitle = AuthError.AuthenticationFailed.ErrorTitleToString(),
                StatusCodes = StatusCodes.Status401Unauthorized
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        // Set Context User
        var contextUser = new ContextUser
        {
            Email = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Email).Value,
            UserId = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Sub).Value
        };
        httpContext.SetUserContext(contextUser);

        await next();
    }
}