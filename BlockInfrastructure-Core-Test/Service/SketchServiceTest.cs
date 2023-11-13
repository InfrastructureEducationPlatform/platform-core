using System.Text.Json;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Test.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Service;

public class SketchServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly SketchService _sketchService;

    public SketchServiceTest()
    {
        _sketchService = new SketchService(_databaseContext);
    }

    [Fact(DisplayName = "ListSketches: ListSketches는 데이터에 포함되어 있는 스케치 리스트를 Sketch Response 형태로 반환합니다.")]
    public async Task Is_ListSketches_Returns_SketchResponseList()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var sketchId = Ulid.NewUlid().ToString();
        var sketch = new Sketch
        {
            Id = sketchId,
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            ChannelId = channelId,
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        _databaseContext.Sketches.Add(sketch);
        await _databaseContext.SaveChangesAsync();

        // Do
        var result = await _sketchService.ListSketches(channelId);

        // Check
        Assert.Single(result);
        Assert.Equal(sketchId, result[0].SketchId);
        Assert.Equal(sketch.Name, result[0].Name);
        Assert.Equal(sketch.Description, result[0].Description);
        Assert.Equal(sketch.ChannelId, result[0].ChannelId);
    }
}