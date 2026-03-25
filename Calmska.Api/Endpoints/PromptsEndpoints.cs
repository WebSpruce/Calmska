using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Prompts.Queries;
using MediatR;
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
            ISender sender,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            if (string.IsNullOrEmpty(request.Prompt))
                return Results.BadRequest("Prompt cannot be empty.");
            
            var query = new GetPromptResponseCommand(request.Prompt, request.IsAnalize, request.IsMoodEntry);
            
            var result = await sender.Send(query, token);
            return Results.Ok(new PromptResponse() { Result = result });
        });
    }
}