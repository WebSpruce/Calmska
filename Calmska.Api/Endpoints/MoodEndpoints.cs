using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Moods.Commands;
using Calmska.Application.Features.Moods.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class MoodEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var mood = app
            .MapGroup(ApiRoutes.Moods.GroupName)
            .WithTags("Moods")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        mood.MapGet("/", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
        });
        mood.MapGet("/searchList", async (ISender sender, [FromQuery] Guid? moodId, [FromQuery] string? moodName, [FromQuery] int? type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetAllByArgumentQuery(moodId, moodName, type ?? 0, pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
        });
        mood.MapGet("/search", async (ISender sender, 
            [FromQuery] Guid? moodId, [FromQuery] string? moodName, [FromQuery] int? type,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new GetByArgumentQuery(moodId, moodName, type ?? 0);
            
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Mood not found");
        });
        mood.MapPost("/", async (ISender sender,
            [FromBody] MoodDTO moodDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new CreateCommand(moodDto.MoodName ?? "", moodDto.MoodTypeId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Created($"/{moodDto.MoodId}", moodDto) : Results.BadRequest(result.Error);
        });
        mood.MapPut("/", async (ISender sender,
            [FromBody] MoodDTO moodDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new UpdateCommand(moodDto.MoodId, moodDto.MoodName, moodDto.MoodTypeId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Mood updated successfully") : Results.BadRequest(result.Error);
        });
        mood.MapDelete("/", async (ISender sender, [FromBody] Guid moodId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new DeleteCommand(moodId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Mood deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}