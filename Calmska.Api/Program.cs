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
            var accounts = app.MapGroup("/api/v1/accounts");
            accounts.MapGet("/", async (IAccountRepository accountRepository) =>
            {
                var result = await accountRepository.GetAllAsync();
                return Results.Ok(result);
            });
            accounts.MapPost("/searchList", async (IAccountRepository accountRepository, [FromBody] Account account) =>
            {
                var result = await accountRepository.GetAllByArgumentAsync(account);
                return result != null ? Results.Ok(result) : Results.NotFound("Accounts not found");
            });
            accounts.MapPost("/search", async (IAccountRepository accountRepository, [FromBody] Account account) =>
            {
                var result = await accountRepository.GetByArgumentAsync(account);
                return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
            });
            accounts.MapPost("/", async (IAccountRepository accountRepository, [FromBody] AccountDTO account) =>
            {
                var result = await accountRepository.AddAsync(account);
                return result.Result ? Results.Created($"/{account.UserId}", account) : Results.BadRequest(result.Error);
            });
            accounts.MapPut("/", async (IAccountRepository accountRepository, [FromBody] AccountDTO account) =>
            {
                var result = await accountRepository.UpdateAsync(account);
                return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
            });
            accounts.MapDelete("/", async (IAccountRepository accountRepository, [FromBody] Account account) =>
            {
                var result = await accountRepository.DeleteAsync(account);
                return result.Result ? Results.Ok("Account deleted  successfully") : Results.BadRequest(result.Error);
            });

            app.Run();
        }
    }
}
