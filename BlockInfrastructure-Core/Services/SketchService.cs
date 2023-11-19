using System.Text.Json;
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
                                        BlockSketch = sketch.BlockSketch
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
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        databaseContext.Sketches.Add(sketch);
        await databaseContext.SaveChangesAsync();

        return new SketchResponse
        {
            SketchId = sketch.Id,
            Name = sketch.Name,
            Description = sketch.Description,
            ChannelId = sketch.ChannelId,
            BlockSketch = sketch.BlockSketch
        };
    }
}