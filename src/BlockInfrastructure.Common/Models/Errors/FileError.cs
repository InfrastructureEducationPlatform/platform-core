namespace BlockInfrastructure.Common.Models.Errors;

public class FileError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static FileError FileFormatNotSupported => new(ErrorTitle.FileFormatNotSupported);
    public static FileError FileNotFound => new(ErrorTitle.FileNotFound);

    private FileError(ErrorTitle errorTitle)
    {
        _errorTitle = errorTitle;
    }

    public string ErrorTitleToString()
    {
        return _errorTitle.ToString();
    }

    private enum ErrorTitle
    {
        FileFormatNotSupported,
        FileNotFound
    }
}