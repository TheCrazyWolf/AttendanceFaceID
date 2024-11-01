using AttendanceFaceID.Services.Enums;

namespace AttendanceFaceID.Services.Models.Attendances;

public class AttendanceResultDayDetails
{
    public DateOnly Date { get; set; }
    public int TotalHoursLessons { get; set; }
    public AttendanceEnumType AttendanceType { get; set; } 
}