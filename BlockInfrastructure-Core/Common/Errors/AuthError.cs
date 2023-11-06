namespace BlockInfrastructure.Core.Common.Errors;

public class AuthError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static AuthError OAuthFailed => new(ErrorTitle.OAuthFailed);
    public static AuthError JoinTokenValidationFailed => new(ErrorTitle.JoinTokenValidationFailed);
    public static AuthError CredentialAlreadyExists => new(ErrorTitle.CredentialAlreadyExists);

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
        OAuthFailed,

        /// <summary>
        ///     Join Token JWT Validation에 실패했을 때
        /// </summary>
        JoinTokenValidationFailed,

        /// <summary>
        ///     이미 같은 인증 정보로 회원 가입 이력이 있는 경우
        /// </summary>
        CredentialAlreadyExists
    }
}