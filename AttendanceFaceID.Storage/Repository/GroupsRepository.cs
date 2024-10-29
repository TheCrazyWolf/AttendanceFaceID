using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Storage.Context;
using Microsoft.EntityFrameworkCore;

namespace AttendanceFaceID.Storage.Repository;

public class GroupsRepository(AttendanceContext ef)
{
    
    public async Task<Group?> GetGroupByIdAsync(long id)
    {
        return await ef.Groups.FirstOrDefaultAsync(x=> x.Id == id);
    }

    public async Task<Group?> GetGroupByNameAsync(string name)
    {
        return await ef.Groups.FirstOrDefaultAsync(x=> x.Name == name);
    }

    public async Task CreateGroupAsync(Group group)
    {
        ef.Groups.Add(group);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateGroupAsync(Group group)
    {
        ef.Groups.Update(group);
        await ef.SaveChangesAsync();
    }

    public async Task DeleteGroupAsync(Group group)
    {
        ef.Groups.Remove(group);
        await ef.SaveChangesAsync();
    }

    public async Task<IList<Group>> GetGroup()
    {
        return await ef.Groups.ToListAsync();
    }
}