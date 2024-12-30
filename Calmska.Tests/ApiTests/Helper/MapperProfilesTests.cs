namespace Calmska.Tests.ApiTests.Helper
{
    public class MapperProfilesTests
    {
        private readonly IMapper _mapper;
        public MapperProfilesTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfiles>();
            });
            _mapper = config.CreateMapper();
        }
        [Fact]
        public void MapperConfiguration_IsValid()
        {
            var config = (MapperConfiguration)_mapper.ConfigurationProvider;
            config.AssertConfigurationIsValid();
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
