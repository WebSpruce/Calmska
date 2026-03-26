using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Tips.Commands;
using Calmska.Application.Features.Tips.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class TipsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var tips = app
            .MapGroup(ApiRoutes.Tips.GroupName)
            .WithTags("Tips")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        tips.MapGet("/", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
        });
        tips.MapGet("/searchList", async (ISender sender, [FromQuery] Guid? tipId, [FromQuery] string? content, [FromQuery] int? type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new GetAllByArgumentQuery(tipId, content, type, pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
        });
        tips.MapGet("/search", async (ISender sender, 
            [FromQuery] Guid? tipId, [FromQuery] string? content, [FromQuery] int? type,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
           
            var query = new GetByArgumentQuery(tipId, content, type);
            
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
        });
        tips.MapPost("/", async (ISender sender,
            [FromBody] TipsDTO tipsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new CreateCommand(tipsDto.TipId ?? Guid.Empty, tipsDto.Content ?? "", tipsDto.TipsTypeId ?? 0);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Created($"/{tipsDto.TipId}", tipsDto) : Results.BadRequest(result.Error);
        });
        tips.MapPut("/", async (ISender sender,
            [FromBody] TipsDTO tipsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new UpdateCommand(tipsDto.TipId ?? Guid.Empty, tipsDto.Content ?? "", tipsDto.TipsTypeId ?? 0);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Tip updated successfully") : Results.BadRequest(result.Error);
        });
        tips.MapDelete("/", async (ISender sender, [FromBody] Guid tipId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new DeleteCommand(tipId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Tip deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}