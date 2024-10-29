using AttendanceFaceID.Models.Common;

namespace AttendanceFaceID.Models.DLA;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}