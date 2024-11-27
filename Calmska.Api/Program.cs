using Calmska.Api.DTO;
using Calmska.Api.Interfaces;
using Calmska.Api.Repository;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calmska.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();
            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

            builder.Services.AddDbContext<CalmskaDbContext>(options => 
            options.UseMongoDB(mongoDBSettings?.AtlasURI ?? "", mongoDBSettings?.DatabaseName ?? string.Empty));

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //endpoints
            var accounts = app
                .MapGroup("/api/v1/accounts")
                .WithTags("Accounts");
            accounts.MapGet("/", async (IAccountRepository accountRepository) =>
            {
                var result = await accountRepository.GetAllAsync();
                return Results.Ok(result);
            });
            accounts.MapPost("/searchList", async (IAccountRepository accountRepository, [FromBody] AccountDTO accountDTO) =>
            {
                var result = await accountRepository.GetAllByArgumentAsync(accountDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Accounts not found");
            });
            accounts.MapPost("/search", async (IAccountRepository accountRepository, [FromBody] AccountDTO accountDTO) =>
            {
                var result = await accountRepository.GetByArgumentAsync(accountDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
            });
            accounts.MapPost("/", async (IAccountRepository accountRepository, [FromBody] AccountDTO accountDTO) =>
            {
                var result = await accountRepository.AddAsync(accountDTO);
                return result.Result ? Results.Created($"/{accountDTO.UserId}", accountDTO) : Results.BadRequest(result.Error);
            });
            accounts.MapPut("/", async (IAccountRepository accountRepository, [FromBody] AccountDTO accountDTO) =>
            {
                var result = await accountRepository.UpdateAsync(accountDTO);
                return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
            });
            accounts.MapDelete("/", async (IAccountRepository accountRepository, [FromBody] Guid accountId) =>
            {
                var result = await accountRepository.DeleteAsync(accountId);
                return result.Result ? Results.Ok("Account deleted  successfully") : Results.BadRequest(result.Error);
            });
            
            var settings = app
                .MapGroup("/api/v1/settings")
                .WithTags("Settings");
            settings.MapGet("/", async (ISettingsRepository settingsRepository) =>
            {
                var result = await settingsRepository.GetAllAsync();
                return Results.Ok(result);
            });
            settings.MapPost("/searchList", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.GetAllByArgumentAsync(settingsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Accounts not found");
            });
            settings.MapPost("/search", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.GetByArgumentAsync(settingsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
            });
            settings.MapPost("/", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.AddAsync(settingsDTO);
                return result.Result ? Results.Created($"/{settingsDTO.UserId}", settingsDTO) : Results.BadRequest(result.Error);
            });
            settings.MapPut("/", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.UpdateAsync(settingsDTO);
                return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
            });
            settings.MapDelete("/", async (ISettingsRepository settingsRepository, [FromBody] Guid settingsId) =>
            {
                var result = await settingsRepository.DeleteAsync(settingsId);
                return result.Result ? Results.Ok("Account deleted  successfully") : Results.BadRequest(result.Error);
            });

            app.Run();
        }
    }
}
