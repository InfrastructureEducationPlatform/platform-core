namespace BlockInfrastructure.Core.Common.Errors;

public class AuthError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static AuthError OAuthFailed => new(ErrorTitle.OAuthFailed);

    private AuthError(ErrorTitle errorTitle)
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
        ///     OAuth에서 정보를 불러오던 중 문제가 발생한 경우
        /// </summary>
        OAuthFailed
    }
}