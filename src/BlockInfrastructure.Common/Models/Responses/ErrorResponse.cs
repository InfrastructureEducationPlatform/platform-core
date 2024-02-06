namespace BlockInfrastructure.Common.Models.Responses;

public class ErrorResponse
{
    public int StatusCodes { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorTitle { get; set; }
}