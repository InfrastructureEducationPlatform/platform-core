namespace BlockInfrastructure.Common.Models.Messages;

public class UserActionEvent
{
    /// <summary>
    ///     Actor ID
    /// </summary>
    public string UserId { get; set; }

    public DateTimeOffset ActedAt { get; set; }

    public string ActionName { get; set; }
}