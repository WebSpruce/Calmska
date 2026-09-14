using Calmska.ApiClients.Clients;
using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
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

        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddHttpClient<IAccountApiClient, AccountApiClient>(client =>
        {
            client.BaseAddress = baseUri;
        });
        services.AddHttpClient<IApiClient<SettingsDTO>, SettingsApiClient>(client =>
        {
            client.BaseAddress = baseUri;
        });
        services.AddHttpClient<IApiClient<TipsDTO>, TipsApiClient>(client =>
        {
            client.BaseAddress = baseUri;
        });
        services.AddHttpClient<ITypesApiClient<Types_TipsDTO>, TypesTipsApiClient>(client =>
        {
            client.BaseAddress = baseUri;
        });
        services.AddHttpClient<IApiClient<MoodDTO>, MoodApiClient>(client =>
            client.BaseAddress = baseUri
        );
        services.AddHttpClient<IApiClient<MoodHistoryDTO>, MoodHistoryApiClient>(client =>
            client.BaseAddress = baseUri
        );
        services.AddHttpClient<IAiPromptingApiClient, AiPromptingApiClient>(client =>
            client.BaseAddress = baseUri
        );

        return services;
    }
}