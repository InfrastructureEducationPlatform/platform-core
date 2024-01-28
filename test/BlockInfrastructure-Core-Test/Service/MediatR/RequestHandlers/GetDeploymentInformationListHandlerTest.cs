using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Models.MediatR.Requests;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Services.MediatR.RequestHandlers;
using Moq;

namespace BlockInfrastructure.Core.Test.Service.MediatR.RequestHandlers;

public class GetDeploymentInformationListHandlerTest
{
    private readonly GetDeploymentInformationListHandler _handler;
    private readonly Mock<IDeploymentService> _mockDeploymentService = new();
    private readonly Mock<IUserService> _mockUserService = new();

    public GetDeploymentInformationListHandlerTest()
    {
        _handler = new GetDeploymentInformationListHandler(_mockUserService.Object, _mockDeploymentService.Object);
    }

    [Fact(DisplayName = "Handle: Handle은 UserService와 DeploymentService의 메소드를 적절하게 호출해야 합니다.")]
    public async Task Is_Handle_Call_UserService_And_DeploymentService()
    {
        // Let
        _mockUserService.Setup(a => a.GetMeAsync(It.IsAny<ContextUser>()))
                        .ReturnsAsync(new MeProjection
                        {
                            UserId = Ulid.NewUlid().ToString(),
                            ChannelPermissionList = new List<ChannelPermissionProjection>()
                        });
        _mockDeploymentService.Setup(a => a.ListDeploymentForChannelAsync(It.IsAny<IEnumerable<string>>()))
                              .ReturnsAsync(new List<DeploymentLog>());

        // Do
        await _handler.Handle(new GetDeploymentInformationListForUser
        {
            ContextUser = new ContextUser()
        }, CancellationToken.None);

        // Verify
        _mockDeploymentService.VerifyAll();
        _mockUserService.VerifyAll();
    }
}