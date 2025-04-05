using Calmska.Api.Interfaces;
using Calmska.Api.Repository;
using Calmska.Models.DTO;
using Calmska.Models.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace Calmska.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://localhost:{port}");

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            string atlasURI = Environment.GetEnvironmentVariable("mongoDbUri") ?? string.Empty;
            string dbName = Environment.GetEnvironmentVariable("mongoDbName") ?? string.Empty;
            string firebaseApiKey = Environment.GetEnvironmentVariable("calmska_firebaseApiKey") ?? string.Empty;
            builder.Services.AddDbContext<CalmskaDbContext>(options =>
            options.UseMongoDB(atlasURI, dbName));

            builder.Services.AddScoped<IRepository<Account, AccountDTO>, AccountRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IRepository<Settings, SettingsDTO>, SettingsRepository>();
            builder.Services.AddScoped<IRepository<Mood, MoodDTO>, MoodRepository>();
            builder.Services.AddScoped<IRepository<MoodHistory, MoodHistoryDTO>, MoodHistoryRepository>();
            builder.Services.AddScoped<IRepository<Tips, TipsDTO>, TipsRepository>();
            builder.Services.AddScoped<ITypesRepository<Types_Tips, Types_TipsDTO>, Types_TipsRepository>();
            builder.Services.AddScoped<ITypesRepository<Types_Mood, Types_MoodDTO>, Types_MoodRepository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddAuthorization();
            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = firebaseApiKey,
                AuthDomain = "calmska.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            }));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseForwardedHeaders();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //endpoints
            #region Accounts
            var accounts = app
                .MapGroup("/api/v2/accounts")
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
            accounts.MapGet("/login", async (IAccountRepository accountRepository,
                [FromQuery] Guid? UserId, [FromQuery] string? UserName, [FromQuery] string? Email, [FromQuery] string? PasswordHashed) =>
            {
                var accountDTO = new AccountDTO()
                {
                    UserId = Guid.Empty,
                    UserName = string.Empty,
                    Email = Email,
                    PasswordHashed = PasswordHashed,
                };
                var result = await accountRepository.LoginAsync(accountDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Account does not exist");
            });
            accounts.MapPost("/", async (IRepository<Account, AccountDTO> accountRepository,
                [FromBody] AccountDTO accountDTO) =>
            {
                var result = await accountRepository.AddAsync(accountDTO);
                return result.Result ? Results.Created($"/{accountDTO.UserId}", accountDTO) : Results.BadRequest(result.Error);
            });
            accounts.MapPut("/", async (IRepository<Account, AccountDTO> accountRepository,
                [FromBody] AccountDTO accountDTO) =>
            {
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
                .MapGroup("/api/v2/settings")
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
                    PomodoroBreak = PomodoroBreak.ToString(),
                    PomodoroTimer = PomodoroTimer.ToString(),
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
                    PomodoroBreak = PomodoroBreak.ToString(),
                    PomodoroTimer = PomodoroTimer.ToString(),
                    UserId = UserId,
                };
                var result = await settingsRepository.GetByArgumentAsync(settingsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Setting not found: {result?.error}");
            });
            settings.MapPost("/", async (IRepository<Settings, SettingsDTO> settingsRepository,
                [FromBody] SettingsDTO settingsDTO) =>
            {
                var result = await settingsRepository.AddAsync(settingsDTO);
                return result.Result ? Results.Created($"/api/v2/settings/{settingsDTO.SettingsId}", settingsDTO) : Results.BadRequest(result.Error);
            });
            settings.MapPut("/", async (IRepository<Settings, SettingsDTO> settingsRepository,
                [FromBody] SettingsDTO settingsDTO) =>
            {
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
                .MapGroup("/api/v2/moods")
                .WithTags("Moods");
            mood.MapGet("/", async (IRepository<Mood, MoodDTO> moodRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await moodRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
            });
            mood.MapGet("/searchList", async (IRepository<Mood, MoodDTO> moodRepository, [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] int? Type,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var moodDTO = new MoodDTO()
                {
                    MoodId = MoodId,
                    MoodName = MoodName,
                    MoodTypeId = Type != null ? (int)Type : 0,
                };
                var result = await moodRepository.GetAllByArgumentAsync(moodDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Moods not found: {result?.error}");
            });
            mood.MapGet("/search", async (IRepository<Mood, MoodDTO> moodRepository, 
                [FromQuery] Guid? MoodId, [FromQuery] string? MoodName, [FromQuery] int? Type) =>
            {
                var moodDTO = new MoodDTO()
                {
                    MoodId = MoodId,
                    MoodName = MoodName,
                    MoodTypeId = Type != null ? (int)Type : 0,
                };
                var result = await moodRepository.GetByArgumentAsync(moodDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Mood not found");
            });
            mood.MapPost("/", async (IRepository<Mood, MoodDTO> moodRepository,
                [FromBody] MoodDTO moodDTO) =>
            {
                var result = await moodRepository.AddAsync(moodDTO);
                return result.Result ? Results.Created($"/{moodDTO.MoodId}", moodDTO) : Results.BadRequest(result.Error);
            });
            mood.MapPut("/", async (IRepository<Mood, MoodDTO> moodRepository,
                [FromBody] MoodDTO moodDTO) =>
            {
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
                .MapGroup("/api/v2/moodhistory")
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
                [FromBody] MoodHistoryDTO moodHistoryDTO) =>
            {
                DateTime? parsedDate = null;
                if (!string.IsNullOrEmpty(moodHistoryDTO.Date.ToString()))
                {
                    if (!DateTime.TryParse(moodHistoryDTO.Date.ToString(), out var validDate))
                    {
                        return Results.BadRequest($"Invalid Date format: {moodHistoryDTO.Date.ToString()}");
                    }
                    parsedDate = validDate;
                }
                moodHistoryDTO.Date = parsedDate;
                var result = await moodHistoryRepository.AddAsync(moodHistoryDTO);
                return result.Result ? Results.Created($"/{moodHistoryDTO.UserId}", moodHistoryDTO) : Results.BadRequest(result.Error);
            });
            moodHistory.MapPut("/", async (IRepository<MoodHistory, MoodHistoryDTO> moodHistoryRepository,
                [FromBody] MoodHistoryDTO moodHistoryDTO) =>
            {
                DateTime? parsedDate = null;
                if (!string.IsNullOrEmpty(moodHistoryDTO.Date.ToString()))
                {
                    if (!DateTime.TryParse(moodHistoryDTO.Date.ToString(), out var validDate))
                    {
                        return Results.BadRequest($"Invalid Date format: {moodHistoryDTO.Date.ToString()}");
                    }
                    parsedDate = validDate;
                }
                moodHistoryDTO.Date = parsedDate;
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
                .MapGroup("/api/v2/tips")
                .WithTags("Tips");
            tips.MapGet("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await tipsRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
            });
            tips.MapGet("/searchList", async (IRepository<Tips, TipsDTO> tipsRepository, [FromQuery] Guid? TipId, [FromQuery] string? Content, [FromQuery] int? Type,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var tipsDTO = new TipsDTO()
                {
                    TipId = TipId,
                    Content = Content,
                    TipsTypeId = Type,
                };
                var result = await tipsRepository.GetAllByArgumentAsync(tipsDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Tips not found: {result?.error}");
            });
            tips.MapGet("/search", async (IRepository<Tips, TipsDTO> tipsRepository, 
                [FromQuery] Guid? TipId, [FromQuery] string? Content, [FromQuery] int? Type) =>
            {
                var tipsDTO = new TipsDTO()
                {
                    TipId = TipId,
                    Content = Content,
                    TipsTypeId = Type,
                };
                var result = await tipsRepository.GetByArgumentAsync(tipsDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Tip not found");
            });
            tips.MapPost("/", async (IRepository<Tips, TipsDTO> tipsRepository,
                [FromBody] TipsDTO tipsDTO) =>
            {
                var result = await tipsRepository.AddAsync(tipsDTO);
                return result.Result ? Results.Created($"/{tipsDTO.TipId}", tipsDTO) : Results.BadRequest(result.Error);
            });
            tips.MapPut("/", async (IRepository<Tips, TipsDTO> tipsRepository,
                [FromBody] TipsDTO tipsDTO) =>
            {
                var result = await tipsRepository.UpdateAsync(tipsDTO);
                return result.Result ? Results.Ok("Tip updated successfully") : Results.BadRequest(result.Error);
            });
            tips.MapDelete("/", async (IRepository<Tips, TipsDTO> tipsRepository, [FromBody] Guid tipId) =>
            {
                var result = await tipsRepository.DeleteAsync(tipId);
                return result.Result ? Results.Ok("Tip deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Tips
            #region Types_Tips
            var types_tips = app
                .MapGroup("/api/v2/types_tips")
                .WithTags("Types_Tips");
            types_tips.MapGet("/", async ([FromServices] ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await typesRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
            });
            types_tips.MapGet("/searchList", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository, [FromQuery] int? TypeId, [FromQuery] string? Type,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var types_TipDTO = new Types_TipsDTO()
                {
                    TypeId = TypeId,
                    Type = Type
                };
                var result = await typesRepository.GetAllByArgumentAsync(types_TipDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
            });
            types_tips.MapGet("/search", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository,
                [FromQuery] int? TypeId, [FromQuery] string? Type) =>
            {
                var types_TipDTO = new Types_TipsDTO()
                {
                    TypeId = TypeId,
                    Type = Type
                };
                var result = await typesRepository.GetByArgumentAsync(types_TipDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Types not found");
            });
            types_tips.MapPost("/", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository,
                [FromBody] Types_TipsDTO types_TipsDTO) =>
            {
                var result = await typesRepository.AddAsync(types_TipsDTO);
                return result.Result ? Results.Created($"/{types_TipsDTO.TypeId}", types_TipsDTO) : Results.BadRequest(result.Error);
            });
            types_tips.MapPut("/", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository,
                [FromBody] Types_TipsDTO types_TipsDTO) =>
            {
                var result = await typesRepository.UpdateAsync(types_TipsDTO);
                return result.Result ? Results.Ok("Type updated successfully") : Results.BadRequest(result.Error);
            });
            types_tips.MapDelete("/", async ([FromServices]ITypesRepository<Types_Tips, Types_TipsDTO> typesRepository, [FromBody] int typeId) =>
            {
                var result = await typesRepository.DeleteAsync(typeId);
                return result.Result ? Results.Ok("Type deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Types_Tips
            #region Types_Moods
            var types_mood = app
                .MapGroup("/api/v2/types_moods")
                .WithTags("Types_Moods");
            types_mood.MapGet("/", async ([FromServices] ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository, [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var result = await typesRepository.GetAllAsync(pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
            });
            types_mood.MapGet("/searchList", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository, [FromQuery] int? TypeId, [FromQuery] string? Type,
                [FromQuery] int? pageNumber, [FromQuery] int? pageSize) =>
            {
                var types_MoodDTO = new Types_MoodDTO()
                {
                    TypeId = TypeId,
                    Type = Type
                };
                var result = await typesRepository.GetAllByArgumentAsync(types_MoodDTO, pageNumber, pageSize);
                return result != null && result.TotalCount > 0 ? Results.Ok(result) : Results.NotFound($"Types not found: {result?.error}");
            });
            types_mood.MapGet("/search", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository,
                [FromQuery] int? TypeId, [FromQuery] string? Type) =>
            {
                var types_MoodDTO = new Types_MoodDTO()
                {
                    TypeId = TypeId,
                    Type = Type
                };
                var result = await typesRepository.GetByArgumentAsync(types_MoodDTO);
                return result != null ? Results.Ok(result) : Results.NotFound("Types not found");
            });
            types_mood.MapPost("/", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository,
                [FromBody] Types_MoodDTO types_MoodDTO) =>
            {
                var result = await typesRepository.AddAsync(types_MoodDTO);
                return result.Result ? Results.Created($"/{types_MoodDTO.TypeId}", types_MoodDTO) : Results.BadRequest(result.Error);
            });
            types_mood.MapPut("/", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository,
                [FromBody] Types_MoodDTO types_MoodDTO) =>
            {
                var result = await typesRepository.UpdateAsync(types_MoodDTO);
                return result.Result ? Results.Ok("Type updated successfully") : Results.BadRequest(result.Error);
            });
            types_mood.MapDelete("/", async ([FromServices]ITypesRepository<Types_Mood, Types_MoodDTO> typesRepository, [FromBody] int typeId) =>
            {
                var result = await typesRepository.DeleteAsync(typeId);
                return result.Result ? Results.Ok("Type deleted successfully") : Results.BadRequest(result.Error);
            });
            #endregion Types_Moods

            app.Run();
        }
    }
}
