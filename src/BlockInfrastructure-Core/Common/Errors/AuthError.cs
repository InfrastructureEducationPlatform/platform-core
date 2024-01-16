namespace BlockInfrastructure.Core.Common.Errors;

public class AuthError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static AuthError OAuthFailed => new(ErrorTitle.OAuthFailed);
    public static AuthError JoinTokenValidationFailed => new(ErrorTitle.JoinTokenValidationFailed);
    public static AuthError CredentialAlreadyExists => new(ErrorTitle.CredentialAlreadyExists);
    public static AuthError AuthenticationFailed => new(ErrorTitle.AuthenticationFailed);
    public static AuthError ChannelAuthorizationFailed => new(ErrorTitle.ChannelAuthorizationFailed);
    public static AuthError RefreshInvalidAccessToken => new(ErrorTitle.RefreshInvalidAccessToken);
    public static AuthError InvalidRefreshToken => new(ErrorTitle.InvalidRefreshToken);
    public static AuthError RefreshExpired => new(ErrorTitle.RefreshExpired);

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
        CredentialAlreadyExists,

        /// <summary>
        ///     인증에 실패한 경우
        /// </summary>
        AuthenticationFailed,

        /// <summary>
        ///     인가에 실패한 경우(채널에 충분한 권한이 없는 경우)
        /// </summary>
        ChannelAuthorizationFailed,

        /// <summary>
        ///     (리프레시) 리프레시 할 때 요청한 엑세스 토큰이 잘못된 경우
        /// </summary>
        RefreshInvalidAccessToken,

        /// <summary>
        ///     (리프레시) 리프레시 토큰이 잘못된 경우
        /// </summary>
        InvalidRefreshToken,

        /// <summary>
        ///     (리프레시) 리프레시 토큰이 만료된 경우
        /// </summary>
        RefreshExpired
    }
}