namespace BlockInfrastructure.Core.Common.Errors;

public class ChannelError : IErrorTitle
{
    private readonly ErrorTitle _errorTitle;

    public static ChannelError ChannelNotFound => new(ErrorTitle.ChannelNotFound);

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
        ChannelNotFound
    }
}