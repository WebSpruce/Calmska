namespace Calmska.Domain.Filters;

public class TipsFilter
{
    public Guid? TipId { get; set; }
    public string? Content { get; set; }
    public int? TipsTypeId { get; set; }

    public TipsFilter(Guid? tipId, string? content, int? tipsTypeId)
    {
        TipId = tipId;
        Content = content;
        TipsTypeId = tipsTypeId;
    }
}