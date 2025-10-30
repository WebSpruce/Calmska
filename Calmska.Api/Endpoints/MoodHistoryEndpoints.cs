using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class MoodHistoryEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var moodHistory = app
            .MapGroup(ApiRoutes.MoodHistory.GroupName)
            .WithTags("MoodHistory");
        moodHistory.MapGet("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await moodHistoryRepository.GetAllAsync(pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"MoodHistory not found: {result?.error}");
        });
        moodHistory.MapGet("/searchList", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
            [FromQuery] Guid? moodHistoryId, [FromQuery] string? date, [FromQuery] Guid? userId, [FromQuery] Guid? moodId,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (!DateTime.TryParse(date, out var validDate))
                {
                    return Results.BadRequest($"Invalid Date format: {date}");
                }
                parsedDate = validDate;
            }

            var moodHistoryDto = new MoodHistoryDTO()
            {
                MoodHistoryId = moodHistoryId,
                Date = parsedDate,
                UserId = userId,
                MoodId = moodId,
            };
            var result = await moodHistoryRepository.GetAllByArgumentAsync(moodHistoryDto, pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"MoodHistory not found: {result?.error}");
        });
        moodHistory.MapGet("/search", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
            [FromQuery] Guid? moodHistoryId, [FromQuery] string? date, [FromQuery] Guid? userId, [FromQuery] Guid? moodId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date))
            {
                if (!DateTime.TryParse(date, out var validDate))
                {
                    return Results.BadRequest($"Invalid Date format: {date}");
                }
                parsedDate = validDate;
            }

            var moodHistoryDto = new MoodHistoryDTO()
            {
                MoodHistoryId = moodHistoryId,
                Date = parsedDate,
                UserId = userId,
                MoodId = moodId,
            };
            var result = await moodHistoryRepository.GetByArgumentAsync(moodHistoryDto, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Setting not found");
        });
        moodHistory.MapPost("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
            [FromBody] MoodHistoryDTO moodHistoryDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(moodHistoryDto.Date.ToString()))
            {
                if (!DateTime.TryParse(moodHistoryDto.Date.ToString(), out var validDate))
                {
                    return Results.BadRequest($"Invalid Date format: {moodHistoryDto.Date.ToString()}");
                }
                parsedDate = validDate;
            }
            moodHistoryDto.Date = parsedDate;
            var result = await moodHistoryRepository.AddAsync(moodHistoryDto, token);
            return result.Result ? Results.Created($"/{moodHistoryDto.UserId}", moodHistoryDto) : Results.BadRequest(result.Error);
        });
        moodHistory.MapPut("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
            [FromBody] MoodHistoryDTO moodHistoryDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(moodHistoryDto.Date.ToString()))
            {
                if (!DateTime.TryParse(moodHistoryDto.Date.ToString(), out var validDate))
                {
                    return Results.BadRequest($"Invalid Date format: {moodHistoryDto.Date.ToString()}");
                }
                parsedDate = validDate;
            }
            moodHistoryDto.Date = parsedDate;
            var result = await moodHistoryRepository.UpdateAsync(moodHistoryDto, token);
            return result.Result ? Results.Ok("MoodHistory updated successfully") : Results.BadRequest(result.Error);
        });
        moodHistory.MapDelete("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository, [FromBody] Guid moodHistoryId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await moodHistoryRepository.DeleteAsync(moodHistoryId, token);
            return result.Result ? Results.Ok("MoodHistory deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}