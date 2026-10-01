using Calmska.ApiClients.Clients;
using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.ApiClients;

public static class DependencyInjection
{
    public static IServiceCollection AddCalmskaApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var apiBaseUrl = configuration.GetValue<string>("ApiUrl")
                         ?? throw new InvalidOperationException("Configuration 'ApiUrl' is missing.");

        var baseUri = new Uri(apiBaseUrl, UriKind.Absolute);

        services.AddHttpClient<IHttpClientService, HttpClientService>(client =>
        {
            client.BaseAddress = baseUri;
        });
        services.AddScoped<IAccountApiClient, AccountApiClient>();
        services.AddScoped<ISettingsApiClient<Settings, SettingsDTO>, SettingsApiClient>();
        services.AddScoped<IApiClient<TipsDTO>, TipsApiClient>();
        services.AddScoped<ITypesApiClient<Types_TipsDTO>, TypesTipsApiClient>();
        services.AddScoped<IApiClient<MoodDTO>, MoodApiClient>();
        services.AddScoped<IApiClient<MoodHistoryDTO>, MoodHistoryApiClient>();
        services.AddScoped<IAiPromptingApiClient, AiPromptingApiClient>();

        return services;
    }
}