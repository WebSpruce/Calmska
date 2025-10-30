using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class SettingsEndpoints: IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var settings = app
            .MapGroup(ApiRoutes.Settings.GroupName)
            .WithTags("Settings");
        settings.MapGet("/", async (IRepository<Settings, SettingsDTO> settingsRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await settingsRepository.GetAllAsync(pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Settings not found: {result?.error}");
        });
        settings.MapGet("/searchList", async (IRepository<Settings, SettingsDTO> settingsRepository, 
            [FromQuery] Guid? settingsId, [FromQuery] string ? color, [FromQuery] float? pomodoroTimer, [FromQuery] float? pomodoroBreak, [FromQuery]Guid? userId,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var settingsDto = new SettingsDTO()
            {
                SettingsId = settingsId,
                Color = color,
                PomodoroBreak = pomodoroBreak.ToString(),
                PomodoroTimer = pomodoroTimer.ToString(),
                UserId = userId,
            };
            var result = await settingsRepository.GetAllByArgumentAsync(settingsDto, pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Settings not found: {result?.error}");
        });
        settings.MapGet("/search", async (IRepository<Settings, SettingsDTO> settingsRepository,
            [FromQuery] Guid? settingsId, [FromQuery] string? color, [FromQuery] float? pomodoroTimer, [FromQuery] float? pomodoroBreak, [FromQuery] Guid? userId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var settingsDto = new SettingsDTO()
            {
                SettingsId = settingsId,
                Color = color,
                PomodoroBreak = pomodoroBreak.ToString(),
                PomodoroTimer = pomodoroTimer.ToString(),
                UserId = userId,
            };
            var result = await settingsRepository.GetByArgumentAsync(settingsDto, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Setting not found: {result?.error}");
        });
        settings.MapPost("/", async (IRepository<Settings, SettingsDTO> settingsRepository,
            [FromBody] SettingsDTO settingsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await settingsRepository.AddAsync(settingsDto, token);
            return result.Result ? Results.Created($"/api/v2/settings/{settingsDto.SettingsId}", settingsDto) : Results.BadRequest(result.Error);
        });
        settings.MapPut("/", async (IRepository<Settings, SettingsDTO> settingsRepository,
            [FromBody] SettingsDTO settingsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await settingsRepository.UpdateAsync(settingsDto, token);
            return result.Result ? Results.Ok("Settings updated successfully") : Results.BadRequest(result.Error);
        });
        settings.MapDelete("/", async (IRepository<Settings, SettingsDTO> settingsRepository, [FromBody] Guid settingsId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await settingsRepository.DeleteAsync(settingsId, token);
            return result.Result ? Results.Ok("Setting deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}