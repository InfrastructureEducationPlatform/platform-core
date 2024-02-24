using System.Net;
using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class SketchService(DatabaseContext databaseContext, ISendEndpointProvider sendEndpointProvider)
{
    public async Task<List<SketchResponse>> ListSketches(string channelId)
    {
        return await databaseContext.Sketches
                                    .Where(sketch => sketch.ChannelId == channelId)
                                    .Select(sketch => new SketchResponse
                                    {
                                        SketchId = sketch.Id,
                                        Name = sketch.Name,
                                        Description = sketch.Description,
                                        ChannelId = sketch.ChannelId,
                                        BlockSketch = sketch.BlockSketch,
                                        CreatedAt = sketch.CreatedAt,
                                        UpdatedAt = sketch.UpdatedAt
                                    })
                                    .ToListAsync();
    }

    public async Task<SketchResponse> CreateSketchAsync(string channelId, CreateSketchRequest createSketchRequest)
    {
        // Create sketch
        var sketch = new Sketch
        {
            Id = Ulid.NewUlid().ToString(),
            Name = createSketchRequest.Name,
            Description = createSketchRequest.Description,
            ChannelId = channelId,
            BlockSketch = createSketchRequest.BlockSketch
        };
        databaseContext.Sketches.Add(sketch);
        await databaseContext.SaveChangesAsync();

        return new SketchResponse
        {
            SketchId = sketch.Id,
            Name = sketch.Name,
            Description = sketch.Description,
            ChannelId = sketch.ChannelId,
            BlockSketch = sketch.BlockSketch,
            CreatedAt = sketch.CreatedAt,
            UpdatedAt = sketch.UpdatedAt
        };
    }

    public async Task<SketchResponse> UpdateSketchAsync(string channelId, string sketchId,
                                                        UpdateSketchRequest updateSketchRequest)
    {
        var sketch = await databaseContext.Sketches
                                          .Where(sketch => sketch.ChannelId == channelId)
                                          .FirstOrDefaultAsync(sketch => sketch.Id == sketchId) ??
                     throw new ApiException(HttpStatusCode.NotFound, "해당 스케치를 찾을 수 없습니다.", SketchError.SketchNotFound);

        sketch.BlockSketch = updateSketchRequest.BlockData;
        await databaseContext.SaveChangesAsync();

        return new SketchResponse
        {
            SketchId = sketch.Id,
            Name = sketch.Name,
            Description = sketch.Description,
            ChannelId = sketch.ChannelId,
            BlockSketch = sketch.BlockSketch,
            CreatedAt = sketch.CreatedAt,
            UpdatedAt = sketch.UpdatedAt
        };
    }

    public async Task<SketchResponse> GetSketchAsync(string channelId, string sketchId)
    {
        var sketch = await databaseContext.Sketches
                                          .Where(sketch => sketch.ChannelId == channelId)
                                          .FirstOrDefaultAsync(sketch => sketch.Id == sketchId) ??
                     throw new ApiException(HttpStatusCode.NotFound, "해당 스케치를 찾을 수 없습니다.", SketchError.SketchNotFound);

        return new SketchResponse
        {
            SketchId = sketch.Id,
            Name = sketch.Name,
            Description = sketch.Description,
            ChannelId = sketch.ChannelId,
            BlockSketch = sketch.BlockSketch,
            CreatedAt = sketch.CreatedAt,
            UpdatedAt = sketch.UpdatedAt
        };
    }

    public async Task<DeploymentLog> DeployAsync(string sketchId)
    {
        // Create deployment log
        var sketch = await databaseContext.Sketches
                                          .FirstOrDefaultAsync(sketch => sketch.Id == sketchId) ??
                     throw new ApiException(HttpStatusCode.NotFound, "해당 스케치를 찾을 수 없습니다.", SketchError.SketchNotFound);
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
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = sketch.ChannelId
        };
        databaseContext.DeploymentLogs.Add(deploymentLog);
        await databaseContext.SaveChangesAsync();

        // Send Message
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:deployment.started"));
        await sendEndpoint.Send(new StartDeploymentEvent
        {
            DeploymentLog = deploymentLog
        });

        return deploymentLog;
    }
}