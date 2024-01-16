using System.Net;
using System.Text;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlockInfrastructure.Core.Common;

public class ApiException(HttpStatusCode statusCode, string errorMessage, IErrorTitle errorTitle)
    : Exception
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;
    public string ErrorMessage { get; set; } = errorMessage;
    public IErrorTitle ErrorTitle { get; set; } = errorTitle;
}

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var requestId = context.HttpContext.TraceIdentifier;
        var exception = context.Exception;
        switch (exception)
        {
            case ApiException apiException:
                var errorResponse = new ErrorResponse
                {
                    StatusCodes = (int)apiException.StatusCode,
                    ErrorMessage = apiException.ErrorMessage,
                    ErrorTitle = apiException.ErrorTitle.ErrorTitleToString()
                };
                context.Result = new ObjectResult(errorResponse)
                {
                    StatusCode = (int)apiException.StatusCode
                };
                break;
            default:
                _logger.LogCritical("{errorMessage}", CreateErrorMessage(context.HttpContext, exception));
                _logger.LogInformation("{TraceId} has request: {request}", requestId,
                    context.HttpContext.Request.GetDetails());
                context.Result =
                    new ObjectResult(
                        new ErrorResponse
                        {
                            StatusCodes = StatusCodes.Status500InternalServerError,
                            ErrorMessage = $"Error occurred while processing request {requestId}",
                            ErrorTitle = CommonError.UnknownError.ErrorTitleToString()
                        })
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                break;
        }
    }

    private string CreateErrorMessage(HttpContext httpContext, Exception exception)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"`{httpContext.Request.Method}: {httpContext.Request.GetDisplayUrl()}` 요청 처리 중 문제가 발생했습니다:\n");
        stringBuilder.Append($"Request ID: `{httpContext.TraceIdentifier}`\n");
        stringBuilder.Append($"{exception.GetAllExceptionMessage()}\n");
        stringBuilder.Append($"```{exception.StackTrace}```");
        return stringBuilder.ToString();
    }
}