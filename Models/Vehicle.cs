using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public sealed class Vehicle
{
    public int Id { get; set; }

    [Required, StringLength(12)]
    [Display(Name = "Номерний знак")]
    public string PlateNumber { get; set; } = "";

    [Required, StringLength(80)]
    [Display(Name = "Марка та модель")]
    public string MakeModel { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Власник")]
    public string OwnerName { get; set; } = "";

    [Range(1950, 2100)]
    [Display(Name = "Рік випуску")]
    public int? Year { get; set; }

    public ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
