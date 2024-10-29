using AttendanceFaceID.Models.Common;

namespace AttendanceFaceID.Models.DLA;

public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public long? GroupId { get; set; }
    public Group? Group { get; set; }
}