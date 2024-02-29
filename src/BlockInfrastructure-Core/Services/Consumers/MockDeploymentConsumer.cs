using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services.Consumers;

public class DeploymentRequest
{
    public string SketchId { get; set; }
    public JsonDocument BlockList { get; set; }
    public JsonDocument PluginInstallationInformation { get; set; }
}

public class MockDeploymentConsumer(DatabaseContext databaseContext, IConfiguration configuration, ILogger<MockDeploymentConsumer> logger)
    : IConsumer<StartDeploymentEvent>
{
    public async Task Consume(ConsumeContext<StartDeploymentEvent> context)
    {
        var deploymentLog = (await databaseContext.DeploymentLogs.FindAsync(context.Message.DeploymentLog.Id))!;
        var blockList = context.Message.DeploymentLog.Sketch.BlockSketch.RootElement.GetProperty("blockList").EnumerateArray();
        deploymentLog.DeploymentStatus = DeploymentStatus.Deploying;
        await databaseContext.SaveChangesAsync();

        var pluginInstallation = await databaseContext.PluginInstallations
                                                      .Where(a => a.ChannelId == context.Message.DeploymentLog.ChannelId &&
                                                                  a.PluginId == "aws-static")
                                                      .FirstOrDefaultAsync();

        // Request To server
        var requestBody = new DeploymentRequest
        {
            SketchId = context.Message.DeploymentLog.SketchId,
            BlockList = context.Message.DeploymentLog.Sketch.BlockSketch.RootElement.GetProperty("blockList")
                               .Deserialize<JsonDocument>()!,
            PluginInstallationInformation = pluginInstallation.PluginConfiguration
        };
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(configuration.GetConnectionString("DeploymentPluginConnection"))
        };
        var response = await httpClient.PostAsJsonAsync("/sketch/deployment", requestBody);
        logger.LogInformation(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();


        deploymentLog.DeploymentStatus = DeploymentStatus.Deployed;
        deploymentLog.DeploymentOutput = JsonDocument.Parse(responseBody);
        await databaseContext.SaveChangesAsync();
    }
}