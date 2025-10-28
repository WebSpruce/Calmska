namespace Calmska.Api.Endpoints;

public class ApiRoutes
{
    private const string Base = "/api";
    private const string Version = "v2";
    private const string ApiBase = $"{Base}/{Version}";

    public static class Accounts
    {
        public const string GroupName = $"{ApiBase}/accounts";
    }
    public static class Settings
    {
        public const string GroupName = $"{ApiBase}/settings";
    }
    public static class Moods
    {
        public const string GroupName = $"{ApiBase}/moods";
    }
    public static class MoodHistory
    {
        public const string GroupName = $"{ApiBase}/moodhistory";
    }
    public static class TypesTips
    {
        public const string GroupName = $"{ApiBase}/types_tips";
    }
    public static class TypesMoods
    {
        public const string GroupName = $"{ApiBase}/types_moods";
    }
    
}