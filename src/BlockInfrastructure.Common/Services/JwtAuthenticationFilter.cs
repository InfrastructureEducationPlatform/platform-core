using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlockInfrastructure.Common.Services;

public class JwtAuthenticationFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        if (httpContext.GetUserContext() == null)
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