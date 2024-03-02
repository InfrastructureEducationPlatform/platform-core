namespace BlockInfrastructure.Common.Models.Errors;

public class PluginError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static PluginError PluginNotFound => new(ErrorTitle.PluginNotFound);

    private PluginError(ErrorTitle errorTitle)
    {
        _errorTitle = errorTitle;
    }

    public string ErrorTitleToString()
    {
        return _errorTitle.ToString();
    }

    private enum ErrorTitle
    {
        PluginNotFound
    }
}