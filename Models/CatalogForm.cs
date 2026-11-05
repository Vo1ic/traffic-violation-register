using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public sealed class CatalogForm
{
    public string Kind { get; set; } = "inspectors";
    public int Id { get; set; }

    [Required, StringLength(160, MinimumLength = 2)]
    [Display(Name = "Назва")]
    public string Name { get; set; } = "";

    [Required, StringLength(100)]
    [Display(Name = "Додаткова інформація")]
    public string Sub { get; set; } = "";

    [StringLength(120)]
    [Display(Name = "Опис")]
    public string? Extra { get; set; }

    [StringLength(120)]
    public string? Fourth { get; set; }

    [Range(0, 1_000_000)]
    [Display(Name = "Сума, грн")]
    public decimal? Amount { get; set; }

    [Range(1950, 2100)]
    [Display(Name = "Рік")]
    public int? Year { get; set; }
}

public sealed record CatalogRow(int Id, string Name, string Sub, string? Extra, decimal? Amount, int? Year);
