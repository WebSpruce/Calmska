using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Types_Mood.Commands;
using Calmska.Application.Features.Types_Mood.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class TypesMoodEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var types_mood = app
            .MapGroup(ApiRoutes.TypesMoods.GroupName)
            .WithTags("Types_Moods")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        types_mood.MapGet("/", async ([FromServices] ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_mood.MapGet("/searchList", async ([FromServices]ISender sender, [FromQuery] int? typeId, [FromQuery] string? type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new GetAllByArgumentQuery(typeId, type, pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_mood.MapGet("/search", async ([FromServices]ISender sender,
            [FromQuery] int? typeId, [FromQuery] string? type, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetByArgumentQuery(typeId, type);
            
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Types not found");
        });
        types_mood.MapPost("/", async ([FromServices]ISender sender,
            [FromBody] Types_MoodDTO typesMoodDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new CreateCommand(typesMoodDto.TypeId ?? 0, typesMoodDto.Type ?? "");
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Created($"/{typesMoodDto.TypeId}", typesMoodDto) : Results.BadRequest(result.Error);
        });
        types_mood.MapPut("/", async ([FromServices]ISender sender,
            [FromBody] Types_MoodDTO typesMoodDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new UpdateCommand(typesMoodDto.TypeId ?? 0, typesMoodDto.Type ?? "");
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Type updated successfully") : Results.BadRequest(result.Error);
        });
        types_mood.MapDelete("/", async ([FromServices]ISender sender, [FromBody] int typeId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new DeleteCommand(typeId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Type deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}