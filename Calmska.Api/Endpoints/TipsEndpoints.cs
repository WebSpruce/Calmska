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
        tips.MapGet("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var result = await tipsRepository.GetAllAsync(pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
        });
        tips.MapGet("/searchList", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] Guid? tipId, [FromQuery] string? content, [FromQuery] int? type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var tipsDto = new TipsDTO()
            {
                TipId = tipId,
                Content = content,
                TipsTypeId = type,
            };
            var result = await tipsRepository.GetAllByArgumentAsync(tipsDto, pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
        });
        tips.MapGet("/search", async (IRepository<Tips, TipsDTO> tipsRepository, 
            [FromQuery] Guid? tipId, [FromQuery] string? content, [FromQuery] int? type) =>
        {
            var tipsDto = new TipsDTO()
            {
                TipId = tipId,
                Content = content,
                TipsTypeId = type,
            };
            var result = await tipsRepository.GetByArgumentAsync(tipsDto);
            return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
        });
        tips.MapPost("/", async (IRepository<Tips, TipsDTO> tipsRepository,
            [FromBody] TipsDTO tipsDto) =>
        {
            var result = await tipsRepository.AddAsync(tipsDto);
            return result.Result ? Results.Created($"/{tipsDto.TipId}", tipsDto) : Results.BadRequest(result.Error);
        });
        tips.MapPut("/", async (IRepository<Tips, TipsDTO> tipsRepository,
            [FromBody] TipsDTO tipsDto) =>
        {
            var result = await tipsRepository.UpdateAsync(tipsDto);
            return result.Result ? Results.Ok("Tip updated successfully") : Results.BadRequest(result.Error);
        });
        tips.MapDelete("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromBody] Guid tipId) =>
        {
            var result = await tipsRepository.DeleteAsync(tipId);
            return result.Result ? Results.Ok("Tip deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}