using AttendanceFaceID.Models.Common;

namespace AttendanceFaceID.Models.DLA;

public class Student : BaseEntity
{
    public string ShortName { get; set; } = string.Empty;
    public long? GroupId { get; set; }
    public Group? Group { get; set; }
    
}