using System.ComponentModel.DataAnnotations;

namespace AttendanceFaceID.Services.Enums;

public enum AttendanceEnum
{
    // дист
    [Display(Name = "д/о")]
    Distance,
    // практика
    [Display(Name = "п/п")]
    Practice,
    // явка
    [Display(Name = "явка")]
    Came,
    // не явка
    [Display(Name = "Н")]
    Absent
}