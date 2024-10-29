using AttendanceFaceID.Models.Common;

namespace AttendanceFaceID.Models.DLA;

public class Attendance : BaseEntity
{
    public long? StudentId { get; set; }
    public Student? Student { get; set; }
    
    public DateTime DateTimeJoined { get; set; }
    public string ObjectInizialized { get; set; } = string.Empty;
}