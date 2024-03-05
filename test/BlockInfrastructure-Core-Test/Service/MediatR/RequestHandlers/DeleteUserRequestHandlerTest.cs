using System.Net;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Core.Models.MediatR.Requests;
using BlockInfrastructure.Core.Services;
using Moq;

namespace BlockInfrastructure.Core.Test.Service.MediatR.RequestHandlers;

public class DeleteUserRequestHandlerTest
{
    private readonly DeleteUserRequestHandler _handler;
    private readonly Mock<IChannelService> _mockChannelService;
    private readonly Mock<IUserService> _mockUserService;

    public DeleteUserRequestHandlerTest()
    {
        _mockChannelService = new Mock<IChannelService>();
        _mockUserService = new Mock<IUserService>();
        _handler = new DeleteUserRequestHandler(_mockUserService.Object, _mockChannelService.Object);
    }

    [Fact(DisplayName =
        "Handle: Handle은 만약 채널 삭제 사전 검증 체크에 실패한 경우 ApiException에 BadRequest와 ChannelError.ChannelOwnershipTransferNeededBeforeDelete를 반환해야 합니다.")]
    public async Task Is_Handle_Throw_ApiException_When_ChannelService_CheckUserChannelPermissionForDeleteAsync_Returns_False()
    {
        // Let
        _mockChannelService.Setup(a => a.CheckUserChannelPermissionForDeleteAsync(It.IsAny<string>()))
                           .ReturnsAsync(false);

        // Do
        var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(new DeleteUserRequest
        {
            UserId = "test"
        }, CancellationToken.None));

        // Verify
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(ChannelError.ChannelOwnershipTransferNeededBeforeDelete.ErrorTitleToString(),
            exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "Handle: Handle은 만약 채널 삭제 사전 검증 체크에 성공한 경우 UserService의 DeleteUserAsync를 호출해야 합니다.")]
    public async Task
        Is_Handle_Call_UserService_DeleteUserAsync_When_ChannelService_CheckUserChannelPermissionForDeleteAsync_Returns_True()
    {
        // Let
        _mockChannelService.Setup(a => a.CheckUserChannelPermissionForDeleteAsync(It.IsAny<string>()))
                           .ReturnsAsync(true);

        // Do
        await _handler.Handle(new DeleteUserRequest
        {
            UserId = "test"
        }, CancellationToken.None);

        // Verify
        _mockUserService.Verify(a => a.DeleteUserAsync(It.IsAny<string>()), Times.Once);
    }
}