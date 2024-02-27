using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using MassTransit;

namespace BlockInfrastructure.Core.Services.Consumers;

public class MockDeploymentConsumer(DatabaseContext databaseContext) : IConsumer<StartDeploymentEvent>
{
    public async Task Consume(ConsumeContext<StartDeploymentEvent> context)
    {
        var deploymentLog = (await databaseContext.DeploymentLogs.FindAsync(context.Message.DeploymentLog.Id))!;
        var blockList = context.Message.DeploymentLog.Sketch.BlockSketch.RootElement.GetProperty("blockList").EnumerateArray();

        // Output List
        var blockTypeList = new List<JsonDocument>();

        foreach (var eachBlock in blockList)
        {
            var blockType = eachBlock.GetProperty("type").GetString();

            if (blockType == "virtualMachine")
            {
                blockTypeList.Add(JsonSerializer.SerializeToDocument(new
                {
                    id = eachBlock.GetProperty("id").GetString(),
                    type = "virtualMachine",
                    region = "ap-northeast-2",
                    virtualMachineOutput = new
                    {
                        instanceId = Ulid.NewUlid().ToString(),
                        ipAddress = "192.168.0.109",
                        sshPrivateKey = "a"
                    }
                }));
            }
            else if (blockType == "webServer")
            {
                blockTypeList.Add(JsonSerializer.SerializeToDocument(new
                {
                    id = eachBlock.GetProperty("id").GetString(),
                    type = "virtualMachine",
                    region = "ap-northeast-2",
                    webServerFeatures = new
                    {
                        appName = eachBlock.GetProperty("name").GetString(),
                        publicFQDN = "http://example.com"
                    }
                }));
            }
            else if (blockType == "database")
            {
                blockTypeList.Add(JsonSerializer.SerializeToDocument(new
                {
                    id = eachBlock.GetProperty("id").GetString(),
                    type = "virtualMachine",
                    region = "ap-northeast-2",
                    databaseOutput = new
                    {
                        dbInstanceIdentifier = "example-db",
                        publicFQDN = "example-db.ap-northeast-2.rds.amazonaws.com",
                        databaseUsername = "admin",
                        databasePassword = "admin"
                    }
                }));
            }
        }

        deploymentLog.DeploymentStatus = DeploymentStatus.Deployed;
        deploymentLog.DeploymentOutput = JsonSerializer.SerializeToDocument(blockTypeList);
        await databaseContext.SaveChangesAsync();
    }
}