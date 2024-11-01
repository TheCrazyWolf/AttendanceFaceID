using System.ComponentModel.DataAnnotations;

namespace AttendanceFaceID.Services.Enums;

public enum AttendanceEnumType
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
    Absent,
    // Пар нет
    [Display(Name = "В")]
    NoSchedule,
    
}