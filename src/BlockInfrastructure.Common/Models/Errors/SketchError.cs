namespace BlockInfrastructure.Common.Models.Errors;

public class SketchError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static SketchError SketchNotFound => new(ErrorTitle.SketchNotFound);

    private SketchError(ErrorTitle errorTitle)
    {
        _errorTitle = errorTitle;
    }

    public string ErrorTitleToString()
    {
        return _errorTitle.ToString();
    }

    private enum ErrorTitle
    {
        SketchNotFound
    }
}