using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public sealed class OffenseArticle
{
    public int Id { get; set; }

    [Required, StringLength(24)]
    [Display(Name = "Стаття")]
    public string Code { get; set; } = "";

    [Required, StringLength(160)]
    [Display(Name = "Назва порушення")]
    public string Title { get; set; } = "";

    [Range(0, 1_000_000)]
    [Display(Name = "Штраф, грн")]
    public decimal FineAmount { get; set; }

    [StringLength(30)]
    [Display(Name = "Категорія")]
    public string Category { get; set; } = "Загальна";

    public ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
