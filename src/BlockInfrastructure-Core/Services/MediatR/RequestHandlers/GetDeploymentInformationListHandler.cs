using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Core.Models.MediatR.Requests;
using MediatR;

namespace BlockInfrastructure.Core.Services.MediatR.RequestHandlers;

public class GetDeploymentInformationListHandler(IUserService userService, IDeploymentService deploymentService)
    : IRequestHandler<GetDeploymentInformationListForUser, List<DeploymentLog>>
{
    public async Task<List<DeploymentLog>> Handle(GetDeploymentInformationListForUser request,
                                                  CancellationToken cancellationToken)
    {
        // Get User with channel permissions
        var userProjection = await userService.GetMeAsync(request.ContextUser);
        var allowedChannelList = userProjection.ChannelPermissionList.Select(a => a.ChannelId);

        // Get Deployment List
        return await deploymentService.ListDeploymentForChannelAsync(allowedChannelList);
    }
}