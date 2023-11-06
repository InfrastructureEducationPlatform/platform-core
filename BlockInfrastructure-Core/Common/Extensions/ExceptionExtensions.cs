namespace BlockInfrastructure.Core.Common.Extensions;

public static class ExceptionExtensions
{
    public static string GetAllExceptionMessage(this Exception exception)
    {
        if (exception == null)
        {
            return string.Empty;
        }

        return exception.Message + "\n" + GetAllExceptionMessage(exception.InnerException);
    }
}