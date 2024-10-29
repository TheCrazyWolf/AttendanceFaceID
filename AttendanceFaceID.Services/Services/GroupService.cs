using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Storage;

namespace AttendanceFaceID.Services.Services;

public class GroupService(AttendanceMainRepo repository)
{
    public async Task<IList<Group>> GetGroups()
    {
        return await repository.Groups.GetGroup();
    }
}