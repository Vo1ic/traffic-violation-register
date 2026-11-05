using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public sealed class Inspector
{
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 3, ErrorMessage = "Вкажіть ім'я (3–100 символів).")]
    [Display(Name = "ПІБ")]
    public string FullName { get; set; } = "";

    [Required, StringLength(24)]
    [Display(Name = "Нагрудний номер")]
    public string BadgeNumber { get; set; } = "";

    [Required, StringLength(60)]
    [Display(Name = "Звання")]
    public string Rank { get; set; } = "";

    [Required, StringLength(100)]
    [Display(Name = "Підрозділ")]
    public string Unit { get; set; } = "";

    public ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
