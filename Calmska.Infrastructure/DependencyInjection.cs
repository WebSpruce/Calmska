using Calmska.Application.Abstractions;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Domain.Interfaces;
using Calmska.Infrastructure.Persistence;
using Calmska.Infrastructure.Persistence.AI;
using Calmska.Infrastructure.Persistence.Repositories;
using Calmska.Infrastructure.Persistence.Security;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var atlasUri = configuration["mongoDbUri"];
        var dbName   = configuration["mongoDbName"];

        if (string.IsNullOrEmpty(atlasUri))
            throw new InvalidOperationException("MongoDB atlasURI is not configured.");
        if (string.IsNullOrEmpty(dbName))
            throw new InvalidOperationException("MongoDB dbName is not configured.");

        services.AddDbContext<CalmskaDbContext>(options =>
            options.UseMongoDB(atlasUri, dbName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        services.AddSingleton<IAiFirewallService, AiFirewallService>();
        services.AddHttpClient<IAiPromptingRepository, AiPromptingRepository>();
        services.AddScoped<IRepository<Account, AccountFilter>, AccountRepository>();
        services.AddScoped<IRepository<Settings, SettingsFilter>, SettingsRepository>();
        services.AddScoped<IRepository<Mood, MoodFilter>, MoodRepository>();
        services.AddScoped<IRepository<MoodHistory, MoodHistoryFilter>, MoodHistoryRepository>();
        services.AddScoped<IRepository<Tips, TipsFilter>, TipsRepository>();
        services.AddScoped<ITypesRepository<Types_Tips, Types_TipsFilter>, Types_TipsRepository>();
        services.AddScoped<ITypesRepository<Types_Mood, Types_MoodFilter>, Types_MoodRepository>();
        
        var firebaseApiKey = configuration["calmska_firebaseApiKey"];
        if (string.IsNullOrEmpty(firebaseApiKey))
            throw new InvalidOperationException("Firebase API Key is not configured.");

        services.AddSingleton<IFirebaseAuthClient>(_ =>
            new FirebaseAuthClient(new FirebaseAuthConfig
            {
                ApiKey = firebaseApiKey,
                AuthDomain = "calmska.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            }));

        services.AddScoped<IFirebaseService, FirebaseService>();
        
        services.Configure<AiPromptingOptions>(options =>
        {
            options.ApiKey   = configuration["ai_api_key"]   ?? string.Empty;
            options.ApiHost  = configuration["ai_api_host"]  ?? string.Empty;
            options.ApiModel = configuration["ai_api_model"] ?? string.Empty;
        });

        PromptLoader.ClearCache();
        
        return services;
    }
}