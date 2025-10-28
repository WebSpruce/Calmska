using System.Reflection;
using Calmska.Api.Interfaces;

namespace Calmska.Api.Endpoints;

public static class EndpointExtensions
{
    public static void RegisterModules(this IServiceCollection services)
    {
        var modules = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();

        foreach (var module in modules)
        {
            services.AddSingleton(module);
        }
    }

    public static void MapEndpoints(this WebApplication app)
    {
        var modules = app.Services.GetServices<IModule>();
        foreach (var module in modules)
        {
            module.RegisterEndpoints(app);
        }
    }
}