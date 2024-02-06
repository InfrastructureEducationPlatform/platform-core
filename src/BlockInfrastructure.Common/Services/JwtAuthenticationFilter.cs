using System.Net.Http.Headers;
using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlockInfrastructure.Common.Services;

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