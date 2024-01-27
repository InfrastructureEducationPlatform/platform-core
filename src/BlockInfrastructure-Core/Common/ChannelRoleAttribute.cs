using System.Text;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Common;

public enum ChannelIdGetMode
{
    Route,
    Query
}

public class ChannelRoleAttribute(
    ChannelIdGetMode channelIdGetMode,
    string channelIdMetaName,
    params ChannelPermissionType[] allowedPermissions) : Attribute, IAsyncActionFilter
{
    private HashSet<ChannelPermissionType> AllowedPermissions { get; } = allowedPermissions.ToHashSet();

    private Dictionary<ChannelIdGetMode, Func<HttpContext, string>> ChannelIdGetters { get; } = new()
    {
        {
            ChannelIdGetMode.Route, httpContext => httpContext.GetRouteValue(channelIdMetaName) as string ?? ""
        },
        {
            ChannelIdGetMode.Query, httpContext => httpContext.Request.Query[channelIdMetaName].ToString()
        }
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var channelIdRoute = ChannelIdGetters[channelIdGetMode](httpContext);
        var userContext = httpContext.GetUserContext();
        var databaseContext = httpContext.RequestServices.GetRequiredService<DatabaseContext>();
        var channelPermission = await databaseContext.ChannelPermissions
                                                     .Where(a => a.UserId == userContext.UserId &&
                                                                 a.ChannelId == channelIdRoute)
                                                     .FirstOrDefaultAsync();

        if (channelPermission == null)
        {
            context.Result = new ObjectResult(new ErrorResponse
            {
                ErrorMessage = $"Cannot find channel {channelIdRoute}!",
                StatusCodes = StatusCodes.Status403Forbidden,
                ErrorTitle = AuthError.ChannelAuthorizationFailed.ErrorTitleToString()
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        if (!AllowedPermissions.Contains(channelPermission.ChannelPermissionType))
        {
            context.Result = new ObjectResult(new ErrorResponse
            {
                ErrorMessage =
                    $"User does not have permission to access this channel!(Required: {PermissionToString()}, Actual: {channelPermission.ChannelPermissionType.ToString()})",
                StatusCodes = StatusCodes.Status403Forbidden,
                ErrorTitle = AuthError.ChannelAuthorizationFailed.ErrorTitleToString()
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        await next();
    }

    private string PermissionToString()
    {
        var stringBuilder = new StringBuilder();
        foreach (var permission in AllowedPermissions)
        {
            stringBuilder.Append(permission.ToString());
            stringBuilder.Append(", ");
        }

        return stringBuilder.ToString();
    }
}