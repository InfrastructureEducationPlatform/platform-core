using System.Net;
using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Service;

public class DeploymentServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly DeploymentService _deploymentService;

    public DeploymentServiceTest()
    {
        _deploymentService = new DeploymentService(_databaseContext);
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
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = Ulid.NewUlid().ToString(),
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        var deploymentLog = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            Sketch = sketch,
            Plugin = new Plugin
            {
                Id = Ulid.NewUlid().ToString(),
                Name = "Dummy Plugin",
                Description = "Dummy Plugin",
                SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
                {
                })
            },
            DeploymentStatus = DeploymentStatus.Created
        };
        _databaseContext.DeploymentLogs.Add(deploymentLog);
        await _databaseContext.SaveChangesAsync();

        // Do
        var result = await _deploymentService.GetDeploymentAsync(deploymentLog.Id);

        // Check
        Assert.Equal(deploymentLog.Id, result.Id);
        Assert.Equal(deploymentLog.Sketch.Id, result.Sketch.Id);
        Assert.Equal(deploymentLog.Plugin.Id, result.Plugin.Id);
        Assert.Equal(deploymentLog.DeploymentStatus, result.DeploymentStatus);
    }
}