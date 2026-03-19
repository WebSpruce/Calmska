using Calmska.Api.Interfaces;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class PromptsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var prompts = app
            .MapGroup(ApiRoutes.Prompts.GroupName)
            .WithTags("Prompts")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        prompts.MapPost("/prompt", async (
            [FromBody] PromptRequest request,
            IAiPromptingRepository aiService,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            if (string.IsNullOrEmpty(request.Prompt))
                return Results.BadRequest("Prompt cannot be empty.");

            var result = await aiService.GetPromptResponseAsync(request.Prompt, request.IsAnalize, request.IsMoodEntry, token);
            return Results.Ok(new PromptResponse() { Result = result });
        });
    }
}