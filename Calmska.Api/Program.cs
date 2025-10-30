using Calmska.Api.Endpoints;
using Calmska.Api.Interfaces;
using Calmska.Api.Middlewares;
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
#if !DEBUG
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#endif
#if DEBUG
            builder.WebHost.UseUrls($"http://localhost:{port}");
#endif

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            string atlasURI = Environment.GetEnvironmentVariable("mongoDbUri") ?? string.Empty;
            string dbName = Environment.GetEnvironmentVariable("mongoDbName") ?? string.Empty;
            string firebaseApiKey = Environment.GetEnvironmentVariable("calmska_firebaseApiKey") ?? string.Empty;
            builder.Services.AddDbContext<CalmskaDbContext>(options =>
                {
                    if (string.IsNullOrEmpty(atlasURI))
                        throw new InvalidOperationException("MongoDB atlasURI is not configured.");
                    if (string.IsNullOrEmpty(firebaseApiKey))
                        throw new InvalidOperationException("MongoDB dbName is not configured.");
                    
                    options.UseMongoDB(atlasURI, dbName);
                }
            );

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
            builder.Services.AddSingleton<IFirebaseAuthClient>(sp =>
            {
                if (string.IsNullOrEmpty(firebaseApiKey))
                    throw new InvalidOperationException("Firebase API Key is not configured.");
                
                return new FirebaseAuthClient(new FirebaseAuthConfig()
                {
                    ApiKey = firebaseApiKey,
                    AuthDomain = "calmska.firebaseapp.com",
                    Providers = new FirebaseAuthProvider[]
                    {
                        new EmailProvider()
                    }
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.RegisterModules();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<RequestLoggingMiddleware>();
            
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
            
            app.MapEndpoints(); // it registers every endpoint automatically, from EndpointExtensions.cs

            app.Run();
        }
    }
}
