using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;

namespace BlockInfrastructure.Core.Services.Consumers;

public class UserActionEventConsumer(DatabaseContext databaseContext) : IConsumer<UserActionEvent>
{
    public async Task Consume(ConsumeContext<UserActionEvent> context)
    {
        var userActionEvent = context.Message;
        var userAction = new UserAction
        {
            UserId = userActionEvent.UserId,
            ActionName = userActionEvent.ActionName,
            ActedAt = userActionEvent.ActedAt
        };
        databaseContext.UserActions.Add(userAction);
        await databaseContext.SaveChangesAsync();
    }
}