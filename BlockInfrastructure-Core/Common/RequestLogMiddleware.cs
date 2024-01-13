using System.Text;
using System.Text.Json;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Services;

namespace BlockInfrastructure.Core.Common;

public class RequestLogMiddleware
{
    private readonly ILogger<RequestLogMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public RequestLogMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<RequestLogMiddleware> logger)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Setup Response Stream
        var originalStream = httpContext.Response.Body;
        using var memoryStream = new MemoryStream();
        httpContext.Response.Body = memoryStream;

        JsonDocument? requestBody = null;
        JsonDocument? responseBody = null;

        // Request Content
        if (httpContext.Request.ContentLength > 0)
        {
            httpContext.Request.EnableBuffering();
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var requestBodyStr = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;
            requestBody = JsonDocument.Parse(requestBodyStr);
        }

        var contextUser = httpContext.Items.ContainsKey("ContextUser")
            ? httpContext.GetUserContext()
            : null;

        await _next(httpContext);

        if (httpContext.Response.ContentType?.Contains("application/json") == true)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBodyStr =
                await new StreamReader(memoryStream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            responseBody = JsonDocument.Parse(responseBodyStr);
        }
        else
        {
            _logger.LogInformation("Content Type is not supported: {0}", httpContext.Response.ContentType);
        }

        var requestHeaderDictionary = httpContext.Request.Headers.ToDictionary(a => a.Key, a => a.Value.ToString());
        var responseHeaderDictionary = httpContext.Response.Headers.ToDictionary(a => a.Key, a => a.Value.ToString());
        var requestModel = new RequestLog
        {
            UserId = contextUser?.UserId,
            Scheme = httpContext.Request.Scheme,
            HttpMethod = httpContext.Request.Method,
            Path = httpContext.Request.Path,
            QueryString = httpContext.Request.QueryString.ToString(),
            StatusCode = httpContext.Response.StatusCode,
            RequestBody = requestBody,
            ResponseBody = responseBody,
            RequestHeaders = JsonDocument.Parse(JsonSerializer.Serialize(requestHeaderDictionary)),
            ResponseHeaders = JsonDocument.Parse(JsonSerializer.Serialize(responseHeaderDictionary))
        };

        await using var scope = _serviceProvider.CreateAsyncScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        databaseContext.RequestLogs.Add(requestModel);
        await databaseContext.SaveChangesAsync();

        await memoryStream.CopyToAsync(originalStream);
        httpContext.Response.Body = originalStream;
    }
}