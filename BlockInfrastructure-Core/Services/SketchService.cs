using System.Net;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class SketchService(DatabaseContext databaseContext)
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
}