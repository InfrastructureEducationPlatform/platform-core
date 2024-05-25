using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BlockInfrastructure.Common.Services;

public class UserActionAttribute : Attribute, IAsyncActionFilter
{
    public string ActionName { get; set; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var httpUser = httpContext.GetUserContext();
        var publishEndpoint = httpContext.RequestServices.GetRequiredService<IPublishEndpoint>();

        // Send UserActionEvent
        await publishEndpoint.Publish(new UserActionEvent
        {
            UserId = httpUser?.UserId ?? "No-User",
            ActionName = ActionName,
            ActedAt = DateTimeOffset.UtcNow
        });
        
        await next();
    }
}