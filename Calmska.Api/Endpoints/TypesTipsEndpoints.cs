using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class TypesTipsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var types_tips = app
            .MapGroup(ApiRoutes.TypesTips.GroupName)
            .WithTags("Types_Tips");
        types_tips.MapGet("/", async ([FromServices] ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var result = await typesRepository.GetAllAsync(pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_tips.MapGet("/searchList", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository, [FromQuery] int? TypeId, [FromQuery] string? Type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var typesTipDto = new Types_TipsDTO()
            {
                TypeId = TypeId,
                Type = Type
            };
            var result = await typesRepository.GetAllByArgumentAsync(typesTipDto, pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_tips.MapGet("/search", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository,
            [FromQuery] int? typeId, [FromQuery] string? type) =>
        {
            var typesTipDto = new Types_TipsDTO()
            {
                TypeId = typeId,
                Type = type
            };
            var result = await typesRepository.GetByArgumentAsync(typesTipDto);
            return result != null ? Results.Ok(result) : Results.NotFound("Types not found");
        });
        types_tips.MapPost("/", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository,
            [FromBody] Types_TipsDTO typesTipsDto) =>
        {
            var result = await typesRepository.AddAsync(typesTipsDto);
            return result.Result ? Results.Created($"/{typesTipsDto.TypeId}", typesTipsDto) : Results.BadRequest(result.Error);
        });
        types_tips.MapPut("/", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository,
            [FromBody] Types_TipsDTO typesTipsDto) =>
        {
            var result = await typesRepository.UpdateAsync(typesTipsDto);
            return result.Result ? Results.Ok("Type updated successfully") : Results.BadRequest(result.Error);
        });
        types_tips.MapDelete("/", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository, [FromBody] int typeId) =>
        {
            var result = await typesRepository.DeleteAsync(typeId);
            return result.Result ? Results.Ok("Type deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}