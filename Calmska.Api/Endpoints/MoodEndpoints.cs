using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class MoodEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var mood = app
            .MapGroup(ApiRoutes.Moods.GroupName)
            .WithTags("Moods");
        mood.MapGet("/", async (IRepository<Mood, MoodDTO> moodRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await moodRepository.GetAllAsync(pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
        });
        mood.MapGet("/searchList", async (IRepository<Mood, MoodDTO> moodRepository, [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] int? Type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var moodDto = new MoodDTO()
            {
                MoodId = MoodId,
                MoodName = MoodName,
                MoodTypeId = Type != null ? (int)Type : 0,
            };
            var result = await moodRepository.GetAllByArgumentAsync(moodDto, pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
        });
        mood.MapGet("/search", async (IRepository<Mood, MoodDTO> moodRepository, 
            [FromQuery] Guid? moodId, [FromQuery] string? moodName, [FromQuery] int? type,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var moodDto = new MoodDTO()
            {
                MoodId = moodId,
                MoodName = moodName,
                MoodTypeId = type != null ? (int)type : 0,
            };
            var result = await moodRepository.GetByArgumentAsync(moodDto, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Mood not found");
        });
        mood.MapPost("/", async (IRepository<Mood, MoodDTO> moodRepository,
            [FromBody] MoodDTO moodDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await moodRepository.AddAsync(moodDto, token);
            return result.Result ? Results.Created($"/{moodDto.MoodId}", moodDto) : Results.BadRequest(result.Error);
        });
        mood.MapPut("/", async (IRepository<Mood, MoodDTO> moodRepository,
            [FromBody] MoodDTO moodDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await moodRepository.UpdateAsync(moodDto, token);
            return result.Result ? Results.Ok("Mood updated successfully") : Results.BadRequest(result.Error);
        });
        mood.MapDelete("/", async (IRepository<Mood, MoodDTO> moodRepository, [FromBody] Guid moodId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await moodRepository.DeleteAsync(moodId, token);
            return result.Result ? Results.Ok("Mood deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}