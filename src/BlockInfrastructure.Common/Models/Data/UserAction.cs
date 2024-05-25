namespace BlockInfrastructure.Common.Models.Data;

public class UserAction
{
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    public string UserId { get; set; }
    public User User { get; set; }

    public string ActionName { get; set; }
    public DateTimeOffset ActedAt { get; set; }
}