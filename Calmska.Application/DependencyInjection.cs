using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        
        return services;
    }
}