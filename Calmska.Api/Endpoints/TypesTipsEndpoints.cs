using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Types_Tips.Commands;
using Calmska.Application.Features.Types_Tips.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class TypesTipsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var types_tips = app
            .MapGroup(ApiRoutes.TypesTips.GroupName)
            .WithTags("Types_Tips")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        types_tips.MapGet("/", async ([FromServices] ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_tips.MapGet("/searchList", async ([FromServices]ISender sender, [FromQuery] int? typeId, [FromQuery] string? type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetAllByArgumentQuery(typeId, type, pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_tips.MapGet("/search", async ([FromServices]ISender sender,
            [FromQuery] int? typeId, [FromQuery] string? type, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var typesTipDto = new Types_TipsDTO()
            {
                TypeId = typeId,
                Type = type
            };
            var query = new GetByArgumentQuery(typeId, type);
            
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Types not found");
        });
        types_tips.MapPost("/", async ([FromServices]ISender sender,
            [FromBody] Types_TipsDTO typesTipsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new CreateCommand(typesTipsDto.TypeId ?? 0, typesTipsDto.Type ?? "");
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Created($"/{typesTipsDto.TypeId}", typesTipsDto) : Results.BadRequest(result.Error);
        });
        types_tips.MapPut("/", async ([FromServices]ISender sender,
            [FromBody] Types_TipsDTO typesTipsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new UpdateCommand(typesTipsDto.TypeId ?? 0, typesTipsDto.Type ?? "");
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Type updated successfully") : Results.BadRequest(result.Error);
        });
        types_tips.MapDelete("/", async ([FromServices]ISender sender, [FromBody] int typeId,
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