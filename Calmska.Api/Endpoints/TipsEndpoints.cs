using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class TipsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var tips = app
            .MapGroup("/api/v2/tips")
            .WithTags("Tips");
        tips.MapGet("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await tipsRepository.GetAllAsync(pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
        });
        tips.MapGet("/searchList", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] Guid? tipId, [FromQuery] string? content, [FromQuery] int? type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var tipsDto = new TipsDTO()
            {
                TipId = tipId,
                Content = content,
                TipsTypeId = type,
            };
            var result = await tipsRepository.GetAllByArgumentAsync(tipsDto, pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
        });
        tips.MapGet("/search", async (IRepository<Tips, TipsDTO> tipsRepository, 
            [FromQuery] Guid? tipId, [FromQuery] string? content, [FromQuery] int? type,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var tipsDto = new TipsDTO()
            {
                TipId = tipId,
                Content = content,
                TipsTypeId = type,
            };
            var result = await tipsRepository.GetByArgumentAsync(tipsDto, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
        });
        tips.MapPost("/", async (IRepository<Tips, TipsDTO> tipsRepository,
            [FromBody] TipsDTO tipsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await tipsRepository.AddAsync(tipsDto, token);
            return result.Result ? Results.Created($"/{tipsDto.TipId}", tipsDto) : Results.BadRequest(result.Error);
        });
        tips.MapPut("/", async (IRepository<Tips, TipsDTO> tipsRepository,
            [FromBody] TipsDTO tipsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await tipsRepository.UpdateAsync(tipsDto, token);
            return result.Result ? Results.Ok("Tip updated successfully") : Results.BadRequest(result.Error);
        });
        tips.MapDelete("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromBody] Guid tipId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await tipsRepository.DeleteAsync(tipId, token);
            return result.Result ? Results.Ok("Tip deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}