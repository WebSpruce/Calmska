namespace Calmska.Domain.Filters;

public class Types_MoodFilter
{
    public int? TypeId { get; set; }
    public string? Type { get; set; }

    public Types_MoodFilter(int? typeId, string? type)
    {
        TypeId = typeId;
        Type = type;
    }
}