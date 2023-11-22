using System.Net;
using System.Text.Json;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Requests;
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

    [Fact(DisplayName = "CreateSketchAsync: CreateSketchAsync는 요청에 따라 채널 내에 스케치를 생성합니다.")]
    public async Task Is_CreateSketchAsync_Creates_Sketch_Well()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var sketchRequest = new CreateSketchRequest
        {
            Name = "Test Sketch",
            Description = "Test Sketch Description",
            BlockSketch = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        var result = await _sketchService.CreateSketchAsync(channelId, sketchRequest);

        // Check Result
        Assert.NotNull(result);
        Assert.Equal(sketchRequest.Name, result.Name);
        Assert.Equal(sketchRequest.Description, result.Description);
        Assert.Equal(channelId, result.ChannelId);

        // Check Database
        var sketch = await _databaseContext.Sketches.FirstOrDefaultAsync(s => s.Id == result.SketchId);
        Assert.NotNull(sketch);
        Assert.Equal(sketchRequest.Name, sketch.Name);
        Assert.Equal(sketchRequest.Description, sketch.Description);
        Assert.Equal(channelId, sketch.ChannelId);
    }

    [Fact(DisplayName = "UpdateSketchAsync: UpdateSketchAsync는 만약 스케치를 찾을 수 없는 경우 NotFound 예외를 발생시킵니다.")]
    public async Task Is_UpdateSketchAsync_Throws_NotFound_If_Sketch_Not_Found()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var sketchId = Ulid.NewUlid().ToString();
        var updateSketchRequest = new UpdateSketchRequest
        {
            BlockData = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            await _sketchService.UpdateSketchAsync(channelId, sketchId, updateSketchRequest));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(SketchError.SketchNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "UpdateSketchAsync: UpdateSketchAsync는 만약 스케치를 찾을 수 있는 경우 스케치를 업데이트합니다.")]
    public async Task Is_UpdateSketchAsync_Updates_Sketch_If_Sketch_Found()
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
        var updateSketchRequest = new UpdateSketchRequest
        {
            BlockData = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        var result = await _sketchService.UpdateSketchAsync(channelId, sketchId, updateSketchRequest);

        // Check Result
        Assert.NotNull(result);
        Assert.Equal(sketchId, result.SketchId);
        Assert.Equal(sketch.Name, result.Name);
        Assert.Equal(sketch.Description, result.Description);
        Assert.Equal(sketch.ChannelId, result.ChannelId);

        // Check Database
        var updatedSketch = await _databaseContext.Sketches.FirstOrDefaultAsync(s => s.Id == result.SketchId);
        Assert.NotNull(updatedSketch);
        Assert.Equal(sketchId, updatedSketch.Id);
        Assert.Equal(sketch.Name, updatedSketch.Name);
        Assert.Equal(sketch.Description, updatedSketch.Description);
        Assert.Equal(sketch.ChannelId, updatedSketch.ChannelId);
    }

    [Fact(DisplayName = "GetSketchAsync: GetSketchAsync는 만약 스케치를 찾을 수 없는 경우 NotFound 예외를 발생시킵니다.")]
    public async Task Is_GetSketchAsync_Throws_NotFound_If_Sketch_Not_Found()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var sketchId = Ulid.NewUlid().ToString();

        // Do
        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            await _sketchService.GetSketchAsync(channelId, sketchId));

        // Check
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(SketchError.SketchNotFound.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "GetSketchAsync: GetSketchAsync는 만약 스케치를 찾을 수 있는 경우 스케치를 반환합니다.")]
    public async Task Is_GetSketchAsync_Returns_Sketch_If_Sketch_Found()
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
        var result = await _sketchService.GetSketchAsync(channelId, sketchId);

        // Check Result
        Assert.NotNull(result);
        Assert.Equal(sketchId, result.SketchId);
        Assert.Equal(sketch.Name, result.Name);
        Assert.Equal(sketch.Description, result.Description);
        Assert.Equal(sketch.ChannelId, result.ChannelId);
    }
}