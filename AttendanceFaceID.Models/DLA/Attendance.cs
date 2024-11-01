using System.ComponentModel.DataAnnotations.Schema;
using AttendanceFaceID.Models.Common;
using AttendanceFaceID.Models.Enums;

namespace AttendanceFaceID.Models.DLA;

public class Attendance : BaseEntity
{
    public long? StudentId { get; set; }
    public Student? Student { get; set; }

    public DateTime DateTimeJoined { get; set; }
    public long? StationId { get; set; }
    [ForeignKey("StationId")] public Station? Station { get; set; }
    public AttendanceEnum ModeType { get; set; }
}