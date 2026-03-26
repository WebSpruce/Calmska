using Calmska.Api.Interfaces;
using Calmska.Application.DTO;
using Calmska.Application.Features.Accounts.Commands;
using Calmska.Application.Features.Accounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class AccountsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var accounts = app
            .MapGroup(ApiRoutes.Accounts.GroupName)
            .WithTags("Accounts")
            .WithApiVersionSet(ApiRoutes.ApiVersion(app));
        
        accounts.MapGet("/", async (ISender sender, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var query = new GetAllQuery(pageNumber, pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
        });
        
        accounts.MapGet("/searchList", async(ISender sender,
            [FromQuery] Guid ? userId, [FromQuery] string ? userName, [FromQuery] string ? email, [FromQuery] string? passwordHashed, 
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var query = new GetAllByArgumentQuery(
                userId,
                userName,
                email,
                passwordHashed,
                pageNumber,
                pageSize);
            
            var result = await sender.Send(query, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
        });
        
        accounts.MapGet("/search", async (ISender sender,
            [FromQuery] Guid? userId, [FromQuery] string? userName, [FromQuery] string? email, [FromQuery] string? passwordHashed,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new GetByArgumentQuery(
                userId,
                userName,
                email,
                passwordHashed
            );
            var result = await sender.Send(query, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
        });
        
        accounts.MapGet("/login", async (ISender sender,
            [FromQuery] string email, [FromQuery] string password,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var query = new LoginCommand(email, password);
            
            var result = await sender.Send(query, token);
            return result ? Results.Ok(result) : Results.NotFound("Account does not exist");
        });
        
        accounts.MapPost("/", async (ISender sender,
            [FromBody] AccountDTO accountDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var command = new CreateCommand(accountDto.UserName, accountDto.Email, accountDto.PasswordHashed);
            
            var result = await sender.Send(command, token);
            return result.Result ? Results.Created($"/{accountDto.UserId}", accountDto) : Results.BadRequest(result.Error);
        });
        
        accounts.MapPut("/", async (ISender sender,
            [FromBody] AccountDTO accountDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);

            var command = new UpdateCommand(accountDto.UserId, accountDto.UserName, accountDto.Email, accountDto.PasswordHashed);
            
            var result = await sender.Send(command, token);
            return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
        });
        
        accounts.MapDelete("/", async (ISender sender, [FromQuery] Guid accountId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var command = new DeleteCommand(accountId);
            
            var result = await sender.Send(command, token);
            return result.Result ? Results.Ok("Account deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}