using System.ComponentModel.DataAnnotations.Schema;
using AttendanceFaceID.Models.Common;

namespace AttendanceFaceID.Models.DLA;

public class UnificationProfile : BaseEntity
{
    public long? MainProfileId { get; set; }
    [ForeignKey("MainProfileId")] public Student? MainProfile { get; set; }
    public long? SubProfileId { get; set; }
    [ForeignKey("SubProfileId")] public Student? SubProfile { get; set; }
}