using System.ComponentModel.DataAnnotations;

namespace AttendanceFaceID.Models.Common;

public class BaseEntity
{
    [Key] public long Id { get; set; }
}