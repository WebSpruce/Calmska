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
            builder.Services.AddScoped<IMoodRepository, MoodRepository>();
            builder.Services.AddScoped<IMoodHistoryRepository, MoodHistoryRepository>();
            builder.Services.AddScoped<ITipsRepository, TipsRepository>();

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
            #region Accounts
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
                return result.Result ? Results.Ok("Account deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Accounts
            #region Settings
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
                return result != null ? Results.Ok(result) : Results.NotFound("Settings not found");
            });
            settings.MapPost("/search", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.GetByArgumentAsync(settingsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Setting not found");
            });
            settings.MapPost("/", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.AddAsync(settingsDTO);
                return result.Result ? Results.Created($"/{settingsDTO.UserId}", settingsDTO) : Results.BadRequest(result.Error);
            });
            settings.MapPut("/", async (ISettingsRepository settingsRepository, [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.UpdateAsync(settingsDTO);
                return result.Result ? Results.Ok("Settings updated successfully") : Results.BadRequest(result.Error);
            });
            settings.MapDelete("/", async (ISettingsRepository settingsRepository, [FromBody] Guid settingsId) =>
            {
                var result = await settingsRepository.DeleteAsync(settingsId);
                return result.Result ? Results.Ok("Setting deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Settings
            #region Mood
            var mood = app
                .MapGroup("/api/v1/moods")
                .WithTags("Moods");
            mood.MapGet("/", async (IMoodRepository moodRepository) =>
            {
                var result = await moodRepository.GetAllAsync();
                return Results.Ok(result);
            });
            mood.MapPost("/searchList", async (IMoodRepository moodRepository, [FromBody] MoodDTO moodDTO) =>
            {
                var result = await moodRepository.GetAllByArgumentAsync(moodDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Moods not found");
            });
            mood.MapPost("/search", async (IMoodRepository moodRepository, [FromBody] MoodDTO moodDTO) =>
            {
                var result = await moodRepository.GetByArgumentAsync(moodDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Mood not found");
            });
            mood.MapPost("/", async (IMoodRepository moodRepository, [FromBody] MoodDTO moodDTO) =>
            {
                var result = await moodRepository.AddAsync(moodDTO);
                return result.Result ? Results.Created($"/{moodDTO.MoodId}", moodDTO) : Results.BadRequest(result.Error);
            });
            mood.MapPut("/", async (IMoodRepository moodRepository, [FromBody] MoodDTO moodDTO) =>
            {
                var result = await moodRepository.UpdateAsync(moodDTO);
                return result.Result ? Results.Ok("Mood updated successfully") : Results.BadRequest(result.Error);
            });
            mood.MapDelete("/", async (IMoodRepository moodRepository, [FromBody] Guid moodId) =>
            {
                var result = await moodRepository.DeleteAsync(moodId);
                return result.Result ? Results.Ok("Mood deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Mood
            #region MoodHistory
            var moodHistory = app
                .MapGroup("/api/v1/moodhistory")
                .WithTags("MoodHistory");
            moodHistory.MapGet("/", async (IMoodHistoryRepository moodHistoryRepository) =>
            {
                var result = await moodHistoryRepository.GetAllAsync();
                return Results.Ok(result);
            });
            moodHistory.MapPost("/searchList", async (IMoodHistoryRepository moodHistoryRepository, [FromBody] MoodHistoryDTO moodHistoryDTO) =>
            {
                var result = await moodHistoryRepository.GetAllByArgumentAsync(moodHistoryDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("MoodHistory not found");
            });
            moodHistory.MapPost("/search", async (IMoodHistoryRepository moodHistoryRepository, [FromBody] MoodHistoryDTO moodHistoryDTO) =>
            {
                var result = await moodHistoryRepository.GetByArgumentAsync(moodHistoryDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Setting not found");
            });
            moodHistory.MapPost("/", async (IMoodHistoryRepository moodHistoryRepository, [FromBody] MoodHistoryDTO moodHistoryDTO) =>
            {
                var result = await moodHistoryRepository.AddAsync(moodHistoryDTO);
                return result.Result ? Results.Created($"/{moodHistoryDTO.UserId}", moodHistoryDTO) : Results.BadRequest(result.Error);
            });
            moodHistory.MapPut("/", async (IMoodHistoryRepository moodHistoryRepository, [FromBody] MoodHistoryDTO moodHistoryDTO) =>
            {
                var result = await moodHistoryRepository.UpdateAsync(moodHistoryDTO);
                return result.Result ? Results.Ok("MoodHistory updated successfully") : Results.BadRequest(result.Error);
            });
            moodHistory.MapDelete("/", async (IMoodHistoryRepository moodHistoryRepository, [FromBody] Guid moodHistoryId) =>
            {
                var result = await moodHistoryRepository.DeleteAsync(moodHistoryId);
                return result.Result ? Results.Ok("MoodHistory deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion MoodHistory
            #region Tips
            var tips = app
                .MapGroup("/api/v1/tips")
                .WithTags("Tips");
            tips.MapGet("/", async (ITipsRepository tipsRepository) =>
            {
                var result = await tipsRepository.GetAllAsync();
                return Results.Ok(result);
            });
            tips.MapPost("/searchList", async (ITipsRepository tipsRepository, [FromBody] TipsDTO tipsDTO) =>
            {
                var result = await tipsRepository.GetAllByArgumentAsync(tipsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
            });
            tips.MapPost("/search", async (ITipsRepository tipsRepository, [FromBody] TipsDTO tipsDTO) =>
            {
                var result = await tipsRepository.GetByArgumentAsync(tipsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
            });
            tips.MapPost("/", async (ITipsRepository tipsRepository, [FromBody] TipsDTO tipsDTO) =>
            {
                var result = await tipsRepository.AddAsync(tipsDTO);
                return result.Result ? Results.Created($"/{tipsDTO.TipId}", tipsDTO) : Results.BadRequest(result.Error);
            });
            tips.MapPut("/", async (ITipsRepository tipsRepository, [FromBody] TipsDTO tipsDTO) =>
            {
                var result = await tipsRepository.UpdateAsync(tipsDTO);
                return result.Result ? Results.Ok("Tip updated successfully") : Results.BadRequest(result.Error);
            });
            tips.MapDelete("/", async (ITipsRepository tipsRepository, [FromBody] Guid tipId) =>
            {
                var result = await tipsRepository.DeleteAsync(tipId);
                return result.Result ? Results.Ok("Tip deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Tips


            app.Run();
        }
    }
}
