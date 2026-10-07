using Calmska.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Calmska.Tests.ApiTests.Infrastructure;

public sealed class CalmskaApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("mongoDbUri", "mongodb://localhost:27017");
        builder.UseSetting("mongoDbName", "calmska-tests");
        builder.UseSetting("calmska_firebaseApiKey", "test");
        builder.UseSetting("ai_api_key", "test");
        builder.UseSetting("ai_api_host", "https://localhost");
        builder.UseSetting("ai_api_model", "test");
        builder.UseSetting("automapper_key", "test");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CalmskaDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<CalmskaDbContext>>();

            services.AddDbContext<CalmskaDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }
}