using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class AccountsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var accounts = app
            .MapGroup(ApiRoutes.Accounts.GroupName)
            .WithTags("Accounts");
        
        accounts.MapGet("/", async (IRepository<Account, AccountDTO> accountRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await accountRepository.GetAllAsync(pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
        });
        
        accounts.MapGet("/searchList", async(IRepository < Account, AccountDTO > accountRepository,
            [FromQuery] Guid ? userId, [FromQuery] string ? userName, [FromQuery] string ? email, [FromQuery] string? passwordHashed, 
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var accountDto = new AccountDTO()
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                PasswordHashed = passwordHashed,
            };
            var result = await accountRepository.GetAllByArgumentAsync(accountDto, pageNumber, pageSize, token);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
        });
        
        accounts.MapGet("/search", async (IRepository<Account, AccountDTO> accountRepository,
            [FromQuery] Guid? userId, [FromQuery] string? userName, [FromQuery] string? email, [FromQuery] string? passwordHashed,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var accountDto = new AccountDTO()
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                PasswordHashed = passwordHashed,
            };
            var result = await accountRepository.GetByArgumentAsync(accountDto, token);
            return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
        });
        
        accounts.MapGet("/login", async (IAccountRepository accountRepository,
            [FromQuery] Guid? userId, [FromQuery] string? userName, [FromQuery] string? email, [FromQuery] string? passwordHashed,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var accountDto = new AccountDTO()
            {
                UserId = Guid.Empty,
                UserName = string.Empty,
                Email = email,
                PasswordHashed = passwordHashed,
            };
            var result = await accountRepository.LoginAsync(accountDto, token);
            return result ? Results.Ok(result) : Results.NotFound("Account does not exist");
        });
        
        accounts.MapPost("/", async (IRepository<Account, AccountDTO> accountRepository,
            [FromBody] AccountDTO accountDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            
            var result = await accountRepository.AddAsync(accountDto, token);
            return result.Result ? Results.Created($"/{accountDto.UserId}", accountDto) : Results.BadRequest(result.Error);
        });
        
        accounts.MapPut("/", async (IRepository<Account, AccountDTO> accountRepository,
            [FromBody] AccountDTO accountDto, CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await accountRepository.UpdateAsync(accountDto, token);
            return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
        });
        
        accounts.MapDelete("/", async (IRepository<Account, AccountDTO> accountRepository, [FromQuery] Guid accountId,
            CancellationToken token) =>
        {
            if (token.IsCancellationRequested)
                return Results.StatusCode(499);
            var result = await accountRepository.DeleteAsync(accountId, token);
            return result.Result ? Results.Ok("Account deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}