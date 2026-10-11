using Calmska.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Calmska.Tests.ApiTests.Infrastructure;

public static class QuerySender
{
    internal static async Task<T> QueryDbAsync<T>(CalmskaApiFactory factory, Func<CalmskaDbContext, Task<T>> query)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CalmskaDbContext>();
        return await query(db);
    }
}