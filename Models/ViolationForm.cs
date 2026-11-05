using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public sealed class ViolationForm
{
    public int Id { get; set; }

    [Required, StringLength(24)]
    [Display(Name = "Номер справи")]
    public string CaseNumber { get; set; } = "";

    [Display(Name = "Дата та час")]
    public DateTime OccurredAt { get; set; } = DateTime.Now;

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

    [Range(1, int.MaxValue, ErrorMessage = "Оберіть транспортний засіб.")]
    [Display(Name = "Транспортний засіб")]
    public int VehicleId { get; set; }

    [MinLength(1, ErrorMessage = "Оберіть хоча б одного інспектора.")]
    [Display(Name = "Інспектори")]
    public List<int> InspectorIds { get; set; } = [];

    [MinLength(1, ErrorMessage = "Оберіть хоча б одну статтю.")]
    [Display(Name = "Статті порушення")]
    public List<int> ArticleIds { get; set; } = [];
}
