using System.Diagnostics.CodeAnalysis;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlockInfrastructure.Core.Common;

[ExcludeFromCodeCoverage]
public class JwtAuthenticationFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var contextUser = httpContext.Items.ContainsKey("ContextUser");

        if (!contextUser)
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

        await next();
    }
}