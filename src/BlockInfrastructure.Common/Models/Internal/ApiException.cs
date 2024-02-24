using System.Net;
using BlockInfrastructure.Common.Models.Errors;

namespace BlockInfrastructure.Common.Models.Internal;

public class ApiException(HttpStatusCode statusCode, string errorMessage, IErrorTitle errorTitle)
    : Exception
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;
    public string ErrorMessage { get; set; } = errorMessage;
    public IErrorTitle ErrorTitle { get; set; } = errorTitle;
}