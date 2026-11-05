using System.ComponentModel.DataAnnotations;

namespace TrafficRegister.Models;

public enum ViolationStatus
{
    [Display(Name = "Нова справа")]
    Registered,
    [Display(Name = "Постанову винесено")]
    Issued,
    [Display(Name = "Оскаржується")]
    Appealed,
    [Display(Name = "Закрито")]
    Closed
}
