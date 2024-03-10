using System.Net;
using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlockInfrastructure.Core.Test.Service;

public class DeploymentServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly IDeploymentService _deploymentService;

    private readonly Mock<ISendEndpointProvider> _sendEndpointProvider = new();

    public DeploymentServiceTest()
    {
        _deploymentService = new DeploymentService(_databaseContext, _sendEndpointProvider.Object);
    }

    [Fact(DisplayName = "GetDeploymentAsync: GetDeploymentAsync는 만약 배포를 찾을 수 없는 경우 ApiException을 던집니다.")]
    public async Task Is_GetDeploymentAsync_Throws_ApiException_When_Deployment_Not_Found()
    {
        // Let
        var deploymentId = Ulid.NewUlid().ToString();

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() => _deploymentService.GetDeploymentAsync(deploymentId));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(DeploymentError.DeploymentNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "GetDeploymentAsync: GetDeploymentAsync는 배포 정보가 정상적으로 있는 경우 DeploymentLog를 반환합니다.")]
    public async Task Is_GetDeploymentAsync_Returns_DeploymentLog_Well()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "TestChannel",
            Description = "TestDescription",
            ProfileImageUrl = null
        };
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = channel.Id,
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        var deploymentLog = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            Sketch = sketch,
            Channel = channel,
            PluginInstallation = new PluginInstallation
            {
                Channel = channel,
                Plugin = new Plugin
                {
                    Id = Ulid.NewUlid().ToString(),
                    Name = "Dummy Plugin",
                    Description = "Dummy Plugin",
                    SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
                    {
                    })
                },
                PluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            },
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = sketch.ChannelId,
            CapturedBlockData = sketch.BlockSketch
        };
        _databaseContext.DeploymentLogs.Add(deploymentLog);
        await _databaseContext.SaveChangesAsync();

        // Do
        var result = await _deploymentService.GetDeploymentAsync(deploymentLog.Id);

        // Check
        Assert.Equal(deploymentLog.Id, result.Id);
        Assert.Equal(deploymentLog.Sketch.Id, result.Sketch.Id);
        Assert.Equal(deploymentLog.PluginInstallation.Plugin.Id, result.PluginInstallation.Plugin.Id);
        Assert.Equal(deploymentLog.DeploymentStatus, result.DeploymentStatus);
    }

    [Fact(DisplayName = "ListDeploymentForChannelAsync: ListDeploymentForChannelAsync는 만약 채널에 배포가 없는 경우 빈 리스트를 반환합니다.")]
    public async Task Is_ListDeploymentForChannelAsync_Returns_Empty_List_When_No_Deployment_Exists()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();

        // Do
        var result = await _deploymentService.ListDeploymentForChannelAsync(new List<string>
        {
            channelId
        });

        // Check
        Assert.Empty(result);
    }

    [Fact(DisplayName = "ListDeploymentForChannelAsync: ListDeploymentForChannelAsync는 만약 채널에 배포가 있는 경우 해당 배포 리스트를 반환합니다.")]
    public async Task Is_ListDeploymentForChannelAsync_Returns_DeploymentList_When_Deployment_Exists()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "TestChannel",
            Description = "TestDescription",
            ProfileImageUrl = null
        };
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = channel.Id,
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        var deploymentLog = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            Sketch = sketch,
            Channel = channel,
            PluginInstallation = new PluginInstallation
            {
                Channel = channel,
                Plugin = new Plugin
                {
                    Id = Ulid.NewUlid().ToString(),
                    Name = "Dummy Plugin",
                    Description = "Dummy Plugin",
                    SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
                    {
                    })
                },
                PluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            },
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = sketch.ChannelId,
            CapturedBlockData = sketch.BlockSketch
        };
        _databaseContext.DeploymentLogs.Add(deploymentLog);
        await _databaseContext.SaveChangesAsync();

        // Do
        var result = await _deploymentService.ListDeploymentForChannelAsync(new List<string>
        {
            channel.Id
        });

        // Check
        Assert.Single(result);
        Assert.Equal(deploymentLog.Id, result.First().Id);
        Assert.Equal(deploymentLog.Sketch.Id, result.First().Sketch.Id);
        Assert.Equal(deploymentLog.PluginInstallation.Plugin.Id, result.First().PluginInstallation.Plugin.Id);
        Assert.Equal(deploymentLog.DeploymentStatus, result.First().DeploymentStatus);
    }

    [Fact(DisplayName = "DestroyDeploymentAsync: DestroyDeploymentAsync는 만약 배포를 찾을 수 없는 경우 ApiException을 던집니다.")]
    public async Task Is_DestroyDeploymentAsync_Throws_ApiException_When_Deployment_Not_Found()
    {
        // Let
        var deploymentId = Ulid.NewUlid().ToString();

        // Do
        var exception =
            await Assert.ThrowsAnyAsync<ApiException>(() => _deploymentService.DestroyDeploymentAsync(deploymentId));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(DeploymentError.DeploymentNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "DestroyDeploymentAsync: DestroyDeploymentAsync는 만약 삭제하려고 하는 배포가 최신이 아닌 경우 ApiException을 던집니다.")]
    public async Task Is_DestroyDeploymentAsync_Throws_ApiException_When_Deployment_Is_Not_Latest()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "TestChannel",
            Description = "TestDescription",
            ProfileImageUrl = null
        };
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = channel.Id,
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        var deploymentLog = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            Sketch = sketch,
            Channel = channel,
            PluginInstallation = new PluginInstallation
            {
                Channel = channel,
                Plugin = new Plugin
                {
                    Id = Ulid.NewUlid().ToString(),
                    Name = "Dummy Plugin",
                    Description = "Dummy Plugin",
                    SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
                    {
                    })
                },
                PluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            },
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = sketch.ChannelId,
            CapturedBlockData = sketch.BlockSketch
        };
        _databaseContext.DeploymentLogs.Add(deploymentLog);
        await _databaseContext.SaveChangesAsync();

        // Do
        var exception =
            await Assert.ThrowsAnyAsync<ApiException>(() =>
                _deploymentService.DestroyDeploymentAsync(Ulid.NewUlid().ToString()));

        // Check
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(DeploymentError.CannotDeleteNonLatestDeployment.ErrorTitleToString(),
            exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "DestroyDeploymentAsync: DestroyDeploymentAsync는 만약 삭제하려고 하는 배포가 최신인 경우 해당 배포를 삭제합니다.")]
    public async Task Is_DestroyDeploymentAsync_Deletes_Deployment_When_Deployment_Is_Latest()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "TestChannel",
            Description = "TestDescription",
            ProfileImageUrl = null
        };
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = channel.Id,
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        var deploymentLog = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            Sketch = sketch,
            Channel = channel,
            PluginInstallation = new PluginInstallation
            {
                Channel = channel,
                Plugin = new Plugin
                {
                    Id = Ulid.NewUlid().ToString(),
                    Name = "Dummy Plugin",
                    Description = "Dummy Plugin",
                    SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
                    {
                    })
                },
                PluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            },
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = sketch.ChannelId,
            CapturedBlockData = sketch.BlockSketch
        };
        _databaseContext.DeploymentLogs.Add(deploymentLog);
        await _databaseContext.SaveChangesAsync();
        var mockSendEndpoint = new Mock<ISendEndpoint>();
        _sendEndpointProvider.Setup(x => x.GetSendEndpoint(new Uri("queue:deployment.destroy")))
                             .ReturnsAsync(mockSendEndpoint.Object);
        mockSendEndpoint.Setup(x => x.Send(It.IsAny<StartDeploymentEvent>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        // Do
        await _deploymentService.DestroyDeploymentAsync(deploymentLog.Id);

        // Verify
        mockSendEndpoint.VerifyAll();
        _sendEndpointProvider.VerifyAll();
    }
}