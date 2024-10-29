using System.ComponentModel.DataAnnotations;

namespace AttendanceFaceID.Models.Enums;

public enum AttendanceEnum
{
    [Display(Name = "Карта")]
    Card,
    [Display(Name = "Лицо")]
    Face,
    [Display(Name = "Неизвестно")]
    Unknow
}