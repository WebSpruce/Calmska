namespace Calmska.Domain.Filters;

public class Types_TipsFilter
{
    public int? TypeId { get; set; }
    public string? Type { get; set; }

    public Types_TipsFilter(int? typeId, string? type)
    {
        TypeId = typeId;
        Type = type;
    }
}