using Calmska.Api.Interfaces;
using Calmska.Api.Repository;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Calmska.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string atlasURI = Environment.GetEnvironmentVariable("mongoDbUri") ?? string.Empty;
            string dbName = Environment.GetEnvironmentVariable("mongoDbName") ?? string.Empty;
            builder.Services.AddDbContext<CalmskaDbContext>(options =>
            options.UseMongoDB(atlasURI, dbName));

            builder.Services.AddScoped<IRepository<Account, AccountDTO>, AccountRepository>();
            builder.Services.AddScoped<IRepository<Settings, SettingsDTO>, SettingsRepository>();
            builder.Services.AddScoped<IRepository<Mood, MoodDTO>, MoodRepository>();
            builder.Services.AddScoped<IRepository<MoodHistory, MoodHistoryDTO>, MoodHistoryRepository>();
            builder.Services.AddScoped<IRepository<Tips, TipsDTO>, TipsRepository>();

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
            accounts.MapGet("/", async (IRepository<Account, AccountDTO> accountRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await accountRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
            });
            accounts.MapGet("/searchList", async(IRepository < Account, AccountDTO > accountRepository,
                [FromQuery] Guid ? UserId, [FromQuery] string ? UserName, [FromQuery] string ? Email, [FromQuery] string? PasswordHashed, 
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var accountDTO = new AccountDTO()
                {
                    UserId = UserId,
                    UserName = UserName,
                    Email = Email,
                    PasswordHashed = PasswordHashed,
                };
                var result = await accountRepository.GetAllByArgumentAsync(accountDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Accounts not found: {result?.error}");
            });
            accounts.MapGet("/search", async (IRepository<Account, AccountDTO> accountRepository,
                [FromQuery] Guid? UserId, [FromQuery] string? UserName, [FromQuery] string? Email, [FromQuery] string? PasswordHashed) =>
            {
                var accountDTO = new AccountDTO()
                {
                    UserId = UserId,
                    UserName = UserName,
                    Email = Email,
                    PasswordHashed = PasswordHashed,
                };
                var result = await accountRepository.GetByArgumentAsync(accountDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Account not found");
            });
            accounts.MapPost("/", async (IRepository<Account, AccountDTO> accountRepository,
                [FromQuery] Guid? UserId, [FromQuery] string? UserName, [FromQuery] string? Email, [FromQuery] string? PasswordHashed) =>
            {
                var accountDTO = new AccountDTO()
                {
                    UserId = UserId,
                    UserName = UserName,
                    Email = Email,
                    PasswordHashed = PasswordHashed,
                };
                var result = await accountRepository.AddAsync(accountDTO);
                return result.Result ? Results.Created($"/{accountDTO.UserId}", accountDTO) : Results.BadRequest(result.Error);
            });
            accounts.MapPut("/", async (IRepository<Account, AccountDTO> accountRepository,
                [FromQuery] Guid? UserId, [FromQuery] string? UserName, [FromQuery] string? Email, [FromQuery] string? PasswordHashed) =>
            {
                var accountDTO = new AccountDTO()
                {
                    UserId = UserId,
                    UserName = UserName,
                    Email = Email,
                    PasswordHashed = PasswordHashed,
                };
                var result = await accountRepository.UpdateAsync(accountDTO);
                return result.Result ? Results.Ok("Account updated successfully") : Results.BadRequest(result.Error);
            });
            accounts.MapDelete("/", async (IRepository<Account, AccountDTO> accountRepository, [FromQuery] Guid accountId) =>
            {
                var result = await accountRepository.DeleteAsync(accountId);
                return result.Result ? Results.Ok("Account deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Accounts
            #region Settings
            var settings = app
                .MapGroup("/api/v1/settings")
                .WithTags("Settings");
            settings.MapGet("/", async (IRepository<Settings, SettingsDTO> settingsRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await settingsRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Settings not found: {result?.error}");
            });
            settings.MapGet("/searchList", async (IRepository<Settings, SettingsDTO> settingsRepository, 
                [FromQuery] Guid? SettingsId, [FromQuery] string ? Color, [FromQuery] float? PomodoroTimer, [FromQuery] float? PomodoroBreak, [FromQuery]Guid? UserId,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var settingsDTO = new SettingsDTO()
                {
                    SettingsId = SettingsId,
                    Color = Color,
                    PomodoroBreak = PomodoroBreak,
                    PomodoroTimer = PomodoroTimer,
                    UserId = UserId,
                };
                var result = await settingsRepository.GetAllByArgumentAsync(settingsDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Settings not found: {result?.error}");
            });
            settings.MapGet("/search", async (IRepository<Settings, SettingsDTO> settingsRepository,
                [FromQuery] Guid? SettingsId, [FromQuery] string? Color, [FromQuery] float? PomodoroTimer, [FromQuery] float? PomodoroBreak, [FromQuery] Guid? UserId) =>
            {
                var settingsDTO = new SettingsDTO()
                {
                    SettingsId = SettingsId,
                    Color = Color,
                    PomodoroBreak = PomodoroBreak,
                    PomodoroTimer = PomodoroTimer,
                    UserId = UserId,
                };
                var result = await settingsRepository.GetByArgumentAsync(settingsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Setting not found");
            });
            settings.MapPost("/", async (IRepository<Settings, SettingsDTO> settingsRepository,
                [FromQuery] Guid? SettingsId, [FromQuery] string? Color, [FromQuery] float? PomodoroTimer, [FromQuery] float? PomodoroBreak, [FromQuery] Guid? UserId) =>
            {
                var settingsDTO = new SettingsDTO()
                {
                    SettingsId = SettingsId,
                    Color = Color,
                    PomodoroBreak = PomodoroBreak,
                    PomodoroTimer = PomodoroTimer,
                    UserId = UserId,
                };
                var result = await settingsRepository.AddAsync(settingsDTO);
                return result.Result ? Results.Created($"/{settingsDTO.UserId}", settingsDTO) : Results.BadRequest(result.Error);
            });
            settings.MapPut("/", async (IRepository<Settings, SettingsDTO> settingsRepository,
                [FromQuery] Guid? SettingsId, [FromQuery] string? Color, [FromQuery] float? PomodoroTimer, [FromQuery] float? PomodoroBreak, [FromQuery] Guid? UserId) =>
            {
                var settingsDTO = new SettingsDTO()
                {
                    SettingsId = SettingsId,
                    Color = Color,
                    PomodoroBreak = PomodoroBreak,
                    PomodoroTimer = PomodoroTimer,
                    UserId = UserId,
                };
                var result = await settingsRepository.UpdateAsync(settingsDTO);
                return result.Result ? Results.Ok("Settings updated successfully") : Results.BadRequest(result.Error);
            });
            settings.MapDelete("/", async (IRepository<Settings, SettingsDTO> settingsRepository, [FromBody] Guid settingsId) =>
            {
                var result = await settingsRepository.DeleteAsync(settingsId);
                return result.Result ? Results.Ok("Setting deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Settings
            #region Mood
            var mood = app
                .MapGroup("/api/v1/moods")
                .WithTags("Moods");
            mood.MapGet("/", async (IRepository<Mood, MoodDTO> moodRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await moodRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
            });
            mood.MapGet("/searchList", async (IRepository<Mood, MoodDTO> moodRepository, [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] string? Type,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var moodDTO = new MoodDTO()
                {
                    MoodId = MoodId,
                    MoodName = MoodName,
                    Type = Type,
                };
                var result = await moodRepository.GetAllByArgumentAsync(moodDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
            });
            mood.MapGet("/search", async (IRepository<Mood, MoodDTO> moodRepository, 
                [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] string? Type) =>
            {
                var moodDTO = new MoodDTO()
                {
                    MoodId = MoodId,
                    MoodName = MoodName,
                    Type = Type,
                };
                var result = await moodRepository.GetByArgumentAsync(moodDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Mood not found");
            });
            mood.MapPost("/", async (IRepository<Mood, MoodDTO> moodRepository,
                [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] string? Type) =>
            {
                var moodDTO = new MoodDTO()
                {
                    MoodId = MoodId,
                    MoodName = MoodName,
                    Type = Type,
                };
                var result = await moodRepository.AddAsync(moodDTO);
                return result.Result ? Results.Created($"/{moodDTO.MoodId}", moodDTO) : Results.BadRequest(result.Error);
            });
            mood.MapPut("/", async (IRepository<Mood, MoodDTO> moodRepository,
                [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] string? Type) =>
            {
                var moodDTO = new MoodDTO()
                {
                    MoodId = MoodId,
                    MoodName = MoodName,
                    Type = Type,
                };
                var result = await moodRepository.UpdateAsync(moodDTO);
                return result.Result ? Results.Ok("Mood updated successfully") : Results.BadRequest(result.Error);
            });
            mood.MapDelete("/", async (IRepository<Mood, MoodDTO> moodRepository, [FromBody] Guid moodId) =>
            {
                var result = await moodRepository.DeleteAsync(moodId);
                return result.Result ? Results.Ok("Mood deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Mood
            #region MoodHistory
            var moodHistory = app
                .MapGroup("/api/v1/moodhistory")
                .WithTags("MoodHistory");
            moodHistory.MapGet("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await moodHistoryRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"MoodHistory not found: {result?.error}");
            });
            moodHistory.MapGet("/searchList", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
                [FromQuery] Guid? MoodHistoryId, [FromQuery] string? Date, [FromQuery] Guid? UserId, [FromQuery] Guid? MoodId,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                DateTime? parsedDate = null;
                if (!string.IsNullOrEmpty(Date))
                {
                    if (!DateTime.TryParse(Date, out var validDate))
                    {
                        return Results.BadRequest($"Invalid Date format: {Date}");
                    }
                    parsedDate = validDate;
                }

                var moodHistoryDTO = new MoodHistoryDTO()
                {
                    MoodHistoryId = MoodHistoryId,
                    Date = parsedDate,
                    UserId = UserId,
                    MoodId = MoodId,
                };
                var result = await moodHistoryRepository.GetAllByArgumentAsync(moodHistoryDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"MoodHistory not found: {result?.error}");
            });
            moodHistory.MapGet("/search", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
                [FromQuery] Guid? MoodHistoryId, [FromQuery] string? Date, [FromQuery] Guid? UserId, [FromQuery] Guid? MoodId) =>
            {
                DateTime? parsedDate = null;
                if (!string.IsNullOrEmpty(Date))
                {
                    if (!DateTime.TryParse(Date, out var validDate))
                    {
                        return Results.BadRequest($"Invalid Date format: {Date}");
                    }
                    parsedDate = validDate;
                }

                var moodHistoryDTO = new MoodHistoryDTO()
                {
                    MoodHistoryId = MoodHistoryId,
                    Date = parsedDate,
                    UserId = UserId,
                    MoodId = MoodId,
                };
                var result = await moodHistoryRepository.GetByArgumentAsync(moodHistoryDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Setting not found");
            });
            moodHistory.MapPost("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
                [FromQuery] Guid? MoodHistoryId, [FromQuery] string? Date, [FromQuery] Guid? UserId, [FromQuery] Guid? MoodId) =>
            {
                DateTime? parsedDate = null;
                if (!string.IsNullOrEmpty(Date))
                {
                    if (!DateTime.TryParse(Date, out var validDate))
                    {
                        return Results.BadRequest($"Invalid Date format: {Date}");
                    }
                    parsedDate = validDate;
                }

                var moodHistoryDTO = new MoodHistoryDTO()
                {
                    MoodHistoryId = MoodHistoryId,
                    Date = parsedDate,
                    UserId = UserId,
                    MoodId = MoodId,
                };
                var result = await moodHistoryRepository.AddAsync(moodHistoryDTO);
                return result.Result ? Results.Created($"/{moodHistoryDTO.UserId}", moodHistoryDTO) : Results.BadRequest(result.Error);
            });
            moodHistory.MapPut("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
                [FromQuery] Guid? MoodHistoryId, [FromQuery] string? Date, [FromQuery] Guid? UserId, [FromQuery] Guid? MoodId) =>
            {
                DateTime? parsedDate = null;
                if (!string.IsNullOrEmpty(Date))
                {
                    if (!DateTime.TryParse(Date, out var validDate))
                    {
                        return Results.BadRequest($"Invalid Date format: {Date}");
                    }
                    parsedDate = validDate;
                }

                var moodHistoryDTO = new MoodHistoryDTO()
                {
                    MoodHistoryId = MoodHistoryId,
                    Date = parsedDate,
                    UserId = UserId,
                    MoodId = MoodId,
                };
                var result = await moodHistoryRepository.UpdateAsync(moodHistoryDTO);
                return result.Result ? Results.Ok("MoodHistory updated successfully") : Results.BadRequest(result.Error);
            });
            moodHistory.MapDelete("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository, [FromBody] Guid moodHistoryId) =>
            {
                var result = await moodHistoryRepository.DeleteAsync(moodHistoryId);
                return result.Result ? Results.Ok("MoodHistory deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion MoodHistory
            #region Tips
            var tips = app
                .MapGroup("/api/v1/tips")
                .WithTags("Tips");
            tips.MapGet("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await tipsRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
            });
            tips.MapGet("/searchList", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] Guid? TipId, [FromQuery] string? Content, [FromQuery] string? Type,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var tipsDTO = new TipsDTO()
                {
                    TipId = TipId,
                    Content = Content,
                    Type = Type,
                };
                var result = await tipsRepository.GetAllByArgumentAsync(tipsDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
            });
            tips.MapGet("/search", async (IRepository<Tips, TipsDTO> tipsRepository, 
                [FromQuery] Guid? TipId, [FromQuery] string? Content, [FromQuery] string? Type) =>
            {
                var tipsDTO = new TipsDTO()
                {
                    TipId = TipId,
                    Content = Content,
                    Type = Type,
                };
                var result = await tipsRepository.GetByArgumentAsync(tipsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
            });
            tips.MapPost("/", async (IRepository<Tips, TipsDTO> tipsRepository,
                [FromQuery] Guid? TipId, [FromQuery] string? Content, [FromQuery] string? Type) =>
            {
                var tipsDTO = new TipsDTO()
                {
                    TipId = TipId,
                    Content = Content,
                    Type = Type,
                };
                var result = await tipsRepository.AddAsync(tipsDTO);
                return result.Result ? Results.Created($"/{tipsDTO.TipId}", tipsDTO) : Results.BadRequest(result.Error);
            });
            tips.MapPut("/", async (IRepository<Tips, TipsDTO> tipsRepository,
                [FromQuery] Guid? TipId, [FromQuery] string? Content, [FromQuery] string? Type) =>
            {
                var tipsDTO = new TipsDTO()
                {
                    TipId = TipId,
                    Content = Content,
                    Type = Type,
                };
                var result = await tipsRepository.UpdateAsync(tipsDTO);
                return result.Result ? Results.Ok("Tip updated successfully") : Results.BadRequest(result.Error);
            });
            tips.MapDelete("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromBody] Guid tipId) =>
            {
                var result = await tipsRepository.DeleteAsync(tipId);
                return result.Result ? Results.Ok("Tip deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Tips


            app.Run();
        }
    }
}
