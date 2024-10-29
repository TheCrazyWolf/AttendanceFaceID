using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Storage.Context;
using Microsoft.EntityFrameworkCore;

namespace AttendanceFaceID.Storage.Repository;

public class StudentsRepository(AttendanceContext ef)
{
    public async Task<Student?> GetStudentByShortName(string shortName)
    {
        return await ef.Students.FirstOrDefaultAsync(s => s.ShortName == shortName);
    }
    
    public async Task<Student?> GetStudentByShortName(string shortName, long groupId)
    {
        return await ef.Students.FirstOrDefaultAsync(s => s.ShortName == shortName && s.GroupId == groupId);
    }
    
    public async Task<Student?> GetStudentByIdA(long id)
    {
        return await ef.Students.FirstOrDefaultAsync(x=> x.Id == id);
    }
    
    public async Task<IList<Student>> GetStudentsFromGroup(long groupdId)
    {
        return await ef.Students.Where(s => s.GroupId == groupdId).ToListAsync();
    }
    
    public async Task AddStudent(Student student)
    {
        await ef.AddAsync(student);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateStudent(Student student)
    {
        ef.Update(student);
        await ef.SaveChangesAsync();
    }

    public async Task RemoveStudent(Student student)
    {
        ef.Remove(student);
        await ef.SaveChangesAsync();
    }
}