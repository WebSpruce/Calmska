using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.MoodHistory.Commands;
using Calmska.Application.Features.MoodHistory.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class MoodHistoryEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var moodHistory = app
            .MapGroup(ApiRoutes.MoodHistorys.GroupName)
            .WithTags("MoodHistory")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        moodHistory.MapGet("/", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"MoodHistory not found: {result?.error}");
        });
        moodHistory.MapGet("/searchList", async (ISender sender,
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
            
            var query = new GetAllByArgumentQuery(moodHistoryId, parsedDate, userId, moodId, pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"MoodHistory not found: {result?.error}");
        });
        moodHistory.MapGet("/search", async (ISender sender,
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

            var query = new GetByArgumentQuery(moodHistoryId, parsedDate, userId, moodId);
            
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Setting not found");
        });
        moodHistory.MapPost("/", async (ISender sender,
            [FromBody] MoodHistoryDTO moodHistoryDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new CreateCommand(moodHistoryDto.UserId ?? Guid.Empty, moodHistoryDto.MoodId ?? Guid.Empty);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Created($"/{moodHistoryDto.UserId}", moodHistoryDto) : Results.BadRequest(result.Error);
        });
        moodHistory.MapPut("/", async (ISender sender,
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
            var query = new UpdateCommand(moodHistoryDto.MoodId, moodHistoryDto.Date, moodHistoryDto.UserId ?? Guid.Empty, moodHistoryDto.MoodId ?? Guid.Empty);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("MoodHistory updated successfully") : Results.BadRequest(result.Error);
        });
        moodHistory.MapDelete("/", async (ISender sender, [FromBody] Guid moodHistoryId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new DeleteCommand(moodHistoryId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("MoodHistory deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}