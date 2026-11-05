namespace TrafficRegister.Models;

public sealed class ViolationList
{
    public List<Violation> Items { get; set; } = [];
    public List<Inspector> Inspectors { get; set; } = [];
    public List<OffenseArticle> Articles { get; set; } = [];
    public int? InspectorId { get; set; }
    public int? ArticleId { get; set; }
    public ViolationStatus? Status { get; set; }
    public string? Query { get; set; }
}
