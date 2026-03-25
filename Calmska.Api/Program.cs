using Asp.Versioning;
using Calmska.Api.Endpoints;
using Calmska.Api.Middlewares;
using Calmska.Application;
using Calmska.Infrastructure;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Scalar.AspNetCore;

namespace Calmska.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddOpenApi("v4");

            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
#if !DEBUG
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#endif
#if DEBUG
            builder.WebHost.UseUrls($"http://localhost:{port}");
#endif

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration); 

            string automapperKey = Environment.GetEnvironmentVariable("automapper_key") ?? string.Empty;
            if(string.IsNullOrEmpty(automapperKey))
                throw new InvalidOperationException("Automapper key is empty.");
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = automapperKey;
            }, AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddAuthorization();

            builder.Services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.RegisterModules();

            var app = builder.Build();

            app.UseMiddleware<RequestLoggingMiddleware>();
            
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Calmska API")
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
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
