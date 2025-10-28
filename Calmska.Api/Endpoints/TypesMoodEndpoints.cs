using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class TypesMoodEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var types_mood = app
            .MapGroup(ApiRoutes.TypesMoods.GroupName)
            .WithTags("Types_Moods");
        types_mood.MapGet("/", async ([FromServices] ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var result = await typesRepository.GetAllAsync(pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_mood.MapGet("/searchList", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository, [FromQuery] int? TypeId, [FromQuery] string? Type,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var typesMoodDto = new Types_MoodDTO()
            {
                TypeId = TypeId,
                Type = Type
            };
            var result = await typesRepository.GetAllByArgumentAsync(typesMoodDto, pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
        });
        types_mood.MapGet("/search", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository,
            [FromQuery] int? typeId, [FromQuery] string? type) =>
        {
            var typesMoodDto = new Types_MoodDTO()
            {
                TypeId = typeId,
                Type = type
            };
            var result = await typesRepository.GetByArgumentAsync(typesMoodDto);
            return result != null ? Results.Ok(result) : Results.NotFound("Types not found");
        });
        types_mood.MapPost("/", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository,
            [FromBody] Types_MoodDTO typesMoodDto) =>
        {
            var result = await typesRepository.AddAsync(typesMoodDto);
            return result.Result ? Results.Created($"/{typesMoodDto.TypeId}", typesMoodDto) : Results.BadRequest(result.Error);
        });
        types_mood.MapPut("/", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository,
            [FromBody] Types_MoodDTO typesMoodDto) =>
        {
            var result = await typesRepository.UpdateAsync(typesMoodDto);
            return result.Result ? Results.Ok("Type updated successfully") : Results.BadRequest(result.Error);
        });
        types_mood.MapDelete("/", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository, [FromBody] int typeId) =>
        {
            var result = await typesRepository.DeleteAsync(typeId);
            return result.Result ? Results.Ok("Type deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}