using Calmska.Application.DTO;
using Calmska.Application.Mapping;
using Calmska.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Calmska.Tests.ApiTests.Helper
{
    public class MapperProfilesTests
    {
        private readonly IMapper _mapper;
        public MapperProfilesTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            
            string automapperKey = config["automapper_key"] ?? string.Empty;

            var mapperConfig = new MapperConfigurationExpression();
            mapperConfig.AddProfile<MapperProfiles>();
            mapperConfig.LicenseKey = automapperKey;

            _mapper = new MapperConfiguration(mapperConfig, NullLoggerFactory.Instance)
                .CreateMapper();
        }
        [Fact]
        public void MapperConfiguration_IsValid()
        {
            var mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<MapperProfiles>();
                    cfg.LicenseKey = Environment.GetEnvironmentVariable("automapper_key") ?? string.Empty;
                },
                NullLoggerFactory.Instance
            );

            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData(typeof(Account), typeof(AccountDTO))]
        [InlineData(typeof(AccountDTO), typeof(Account))]
        [InlineData(typeof(Settings), typeof(SettingsDTO))]
        [InlineData(typeof(SettingsDTO), typeof(Settings))]
        [InlineData(typeof(Mood), typeof(MoodDTO))]
        [InlineData(typeof(MoodDTO), typeof(Mood))]
        [InlineData(typeof(MoodHistory), typeof(MoodHistoryDTO))]
        [InlineData(typeof(MoodHistoryDTO), typeof(MoodHistory))]
        [InlineData(typeof(Tips), typeof(TipsDTO))]
        [InlineData(typeof(TipsDTO), typeof(Tips))]
        public void Mapper_CanMapFromSourceToDestination(Type source, Type destination)
        {
            Action mapping = () => _mapper.Map(Activator.CreateInstance(source), source, destination);

            mapping.Should().NotThrow($"Mapping from {source} to {destination} should be possible");
        }

    }
}
