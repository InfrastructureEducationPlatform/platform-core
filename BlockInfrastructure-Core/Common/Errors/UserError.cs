namespace BlockInfrastructure.Core.Common.Errors;

public class UserError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static UserError UserNotFound => new(ErrorTitle.UserNotFound);

    private UserError(ErrorTitle errorTitle)
    {
        _errorTitle = errorTitle;
    }

    public string ErrorTitleToString()
    {
        return _errorTitle.ToString();
    }

    private enum ErrorTitle
    {
        /// <summary>
        ///     사용자를 어떤 이유로 찾을 수 없을 때.
        /// </summary>
        UserNotFound
    }
}