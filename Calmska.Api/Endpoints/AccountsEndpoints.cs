using Calmska.Api.Interfaces;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calmska.Api.Endpoints;

public class AccountsEndpoints : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var accounts = app
            .MapGroup(ApiRoutes.Accounts.GroupName)
            .WithTags("Accounts");
        
        accounts.MapGet("/", async (IRepository<Account, AccountDTO> accountRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var result = await accountRepository.GetAllAsync(pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
        });
        
        accounts.MapGet("/searchList", async(IRepository < Account, AccountDTO > accountRepository,
            [FromQuery] Guid ? userId, [FromQuery] string ? userName, [FromQuery] string ? email, [FromQuery] string? passwordHashed, 
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
        {
            var accountDto = new AccountDTO()
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                PasswordHashed = passwordHashed,
            };
            var result = await accountRepository.GetAllByArgumentAsync(accountDto, pageNumber, pageSize);
            return result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
        });
        
        accounts.MapGet("/search", async (IRepository<Account, AccountDTO> accountRepository,
            [FromQuery] Guid? userId, [FromQuery] string? userName, [FromQuery] string? email, [FromQuery] string? passwordHashed) =>
        {
            var accountDto = new AccountDTO()
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                PasswordHashed = passwordHashed,
            };
            var result = await accountRepository.GetByArgumentAsync(accountDto);
            return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
        });
        
        accounts.MapGet("/login", async (IAccountRepository accountRepository,
            [FromQuery] Guid? userId, [FromQuery] string? userName, [FromQuery] string? email, [FromQuery] string? passwordHashed) =>
        {
            var accountDto = new AccountDTO()
            {
                UserId = Guid.Empty,
                UserName = string.Empty,
                Email = email,
                PasswordHashed = passwordHashed,
            };
            var result = await accountRepository.LoginAsync(accountDto);
            return result ? Results.Ok(result) : Results.NotFound("Account does not exist");
        });
        
        accounts.MapPost("/", async (IRepository<Account, AccountDTO> accountRepository,
            [FromBody] AccountDTO accountDto) =>
        {
            var result = await accountRepository.AddAsync(accountDto);
            return result.Result ? Results.Created($"/{accountDto.UserId}", accountDto) : Results.BadRequest(result.Error);
        });
        
        accounts.MapPut("/", async (IRepository<Account, AccountDTO> accountRepository,
            [FromBody] AccountDTO accountDto) =>
        {
            var result = await accountRepository.UpdateAsync(accountDto);
            return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
        });
        
        accounts.MapDelete("/", async (IRepository<Account, AccountDTO> accountRepository, [FromQuery] Guid accountId) =>
        {
            var result = await accountRepository.DeleteAsync(accountId);
            return result.Result ? Results.Ok("Account deleted successfully") : Results.BadRequest(result.Error);
        });
    }
}