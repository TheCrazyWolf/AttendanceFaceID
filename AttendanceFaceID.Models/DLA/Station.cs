using AttendanceFaceID.Models.Common;

namespace AttendanceFaceID.Models.DLA;

public class Station : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}