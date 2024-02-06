namespace BlockInfrastructure.Core.Common.Errors;

public class CommonError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static IErrorTitle UnknownError => new CommonError(ErrorTitle.UnknownError);

    private CommonError(ErrorTitle errorTitle)
    {
        _errorTitle = errorTitle;
    }

    public string ErrorTitleToString()
    {
        return _errorTitle.ToString();
    }

    private enum ErrorTitle
    {
        UnknownError
    }
}