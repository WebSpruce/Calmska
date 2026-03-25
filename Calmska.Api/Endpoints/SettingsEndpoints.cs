using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Settings.Commands;
using Calmska.Application.Features.Settings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class SettingsEndpoints: IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var settings = app
            .MapGroup(ApiRoutes.Settings.GroupName)
            .WithTags("Settings")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        settings.MapGet("/", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Settings not found: {result?.error}");
        });
        settings.MapGet("/searchList", async (ISender sender, 
            [FromQuery] Guid? settingsId, [FromQuery] string ? color, [FromQuery] float? pomodoroTimer, [FromQuery] float? pomodoroBreak, [FromQuery]Guid? userId,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetAllByArgumentQuery(settingsId, color, pomodoroTimer.ToString(), pomodoroBreak.ToString(), userId, pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Settings not found: {result?.error}");
        });
        settings.MapGet("/search", async (ISender sender,
            [FromQuery] Guid? settingsId, [FromQuery] string? color, [FromQuery] float? pomodoroTimer, [FromQuery] float? pomodoroBreak, [FromQuery] Guid? userId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetByArgumentQuery(settingsId, color, pomodoroTimer.ToString(), pomodoroBreak.ToString(), userId);
            
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound($"Setting not found");
        });
        settings.MapPost("/", async (ISender sender,
            [FromBody] SettingsDTO settingsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new CreateCommand(settingsDto.SettingsId ?? Guid.Empty, settingsDto.Color ?? "", settingsDto.PomodoroTimer ?? "", settingsDto.PomodoroBreak ?? "", settingsDto.UserId ?? Guid.Empty);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Created($"/api/{ApiRoutes.ApiVersion(app)}/settings/{settingsDto.SettingsId}", settingsDto) : Results.BadRequest(result.Error);
        });
        settings.MapPut("/", async (ISender sender,
            [FromBody] SettingsDTO settingsDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new UpdateCommand(settingsDto.SettingsId ?? Guid.Empty, settingsDto.Color ?? "", settingsDto.PomodoroTimer ?? "", settingsDto.PomodoroBreak ?? "", settingsDto.UserId ?? Guid.Empty);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Settings updated successfully") : Results.BadRequest(result.Error);
        });
        settings.MapDelete("/", async (ISender sender, [FromBody] Guid settingsId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new DeleteCommand(settingsId);
            
            var result = await sender.Send(query, token);
            return result.Result ? Results.Ok("Deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}