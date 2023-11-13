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
}