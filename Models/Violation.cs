using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public sealed class Violation
{
    public int Id { get; set; }

    [Required, StringLength(24)]
    [Display(Name = "Номер справи")]
    public string CaseNumber { get; set; } = "";

    [Display(Name = "Дата та час")]
    public DateTime OccurredAt { get; set; }

    [Required, StringLength(160)]
    [Display(Name = "Місце події")]
    public string Location { get; set; } = "";

    [StringLength(600)]
    [Display(Name = "Обставини")]
    public string Description { get; set; } = "";

    [Required, StringLength(100)]
    [Display(Name = "Особа, яка керувала")]
    public string DriverName { get; set; } = "";

    [Display(Name = "Статус")]
    public ViolationStatus Status { get; set; } = ViolationStatus.Registered;

    [Display(Name = "Транспортний засіб")]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [MinLength(1, ErrorMessage = "Призначте хоча б одного інспектора.")]
    public ICollection<Inspector> Inspectors { get; set; } = new List<Inspector>();

    [MinLength(1, ErrorMessage = "Додайте хоча б одну статтю порушення.")]
    public ICollection<OffenseArticle> Articles { get; set; } = new List<OffenseArticle>();
}
