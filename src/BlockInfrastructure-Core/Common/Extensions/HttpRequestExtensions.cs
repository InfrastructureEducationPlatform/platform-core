using System.Text;

namespace BlockInfrastructure.Core.Common.Extensions;

public static class HttpRequestExtensions
{
    public static string GetDetails(this HttpRequest request)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString.Value}";
        var sbHeaders = new StringBuilder();
        foreach (var header in request.Headers)
        {
            sbHeaders.Append($"{header.Key}: {header.Value}\n");
        }

        var body = "no-body";
        if (request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(request.Body);
            body = sr.ReadToEnd();
        }

        return $"{request.Protocol} {request.Method} {baseUrl}\n\n{sbHeaders}\n{body}";
    }
}