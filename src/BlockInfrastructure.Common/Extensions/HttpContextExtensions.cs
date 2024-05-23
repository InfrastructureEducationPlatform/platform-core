using BlockInfrastructure.Common.Models.Internal;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BlockInfrastructure.Common.Extensions;

public static class HttpContextExtensions
{
    private const string ContextUserKey = "ContextUser";

    public static void SetUserContext(this HttpContext httpContext, ContextUser contextUser)
    {
        httpContext.Items[ContextUserKey] = JsonConvert.SerializeObject(contextUser);
    }

    public static ContextUser? GetUserContext(this HttpContext httpContext)
    {
        return JsonConvert.DeserializeObject<ContextUser>(httpContext.Items[ContextUserKey]?.ToString() ?? "");
    }
}