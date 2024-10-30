namespace AttendanceFaceID.Services.Models.Attendances;

public class AttendanceStudentDetails
{
    public long StudentId { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public IList<AttendanceResultDayDetails> Attendances { get; set; } = new List<AttendanceResultDayDetails>();
    
    public AttendanceStudentDetails(){}

    public AttendanceStudentDetails(long studentId, string shortName)
    {
        StudentId = studentId;
        ShortName = shortName;
    }
}