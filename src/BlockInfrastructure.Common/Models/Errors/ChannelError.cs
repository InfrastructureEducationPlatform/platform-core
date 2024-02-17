namespace BlockInfrastructure.Core.Common.Errors;

public class ChannelError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static ChannelError ChannelNotFound => new(ErrorTitle.ChannelNotFound);
    public static ChannelError ChannelPermissionNotFound => new(ErrorTitle.ChannelPermissionNotFound);
    public static ChannelError CannotChangeOwnRole => new(ErrorTitle.CannotChangeOwnRole);

    private ChannelError(ErrorTitle errorTitle)
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
        ///     채널이 존재하지 않는 경우
        /// </summary>
        ChannelNotFound,

        /// <summary>
        ///     사용자/채널 권한을 찾을 수 없는 경우
        /// </summary>
        ChannelPermissionNotFound,

        /// <summary>
        ///     자신의 권한을 변경할 수 없는 경우
        /// </summary>
        CannotChangeOwnRole
    }
}