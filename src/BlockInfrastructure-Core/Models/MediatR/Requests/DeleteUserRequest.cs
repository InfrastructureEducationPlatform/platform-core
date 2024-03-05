using System.Net;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Core.Services;
using MediatR;

namespace BlockInfrastructure.Core.Models.MediatR.Requests;

public class DeleteUserRequest : IRequest
{
    public string UserId { get; set; }
}

public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest>
{
    private readonly IChannelService _channelService;
    private readonly IUserService _userService;

    public DeleteUserRequestHandler(IUserService userService, IChannelService channelService)
    {
        _userService = userService;
        _channelService = channelService;
    }

    public async Task Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var isDeleteApplicable = await _channelService.CheckUserChannelPermissionForDeleteAsync(request.UserId);
        if (!isDeleteApplicable)
        {
            throw new ApiException(HttpStatusCode.BadRequest,
                "User cannot be deleted because user has active channels. Please transfer ownership of the channels to another user.",
                ChannelError.ChannelOwnershipTransferNeededBeforeDelete);
        }

        await _userService.DeleteUserAsync(request.UserId);
    }
}