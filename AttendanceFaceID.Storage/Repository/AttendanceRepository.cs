using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Storage.Context;
using Microsoft.EntityFrameworkCore;

namespace AttendanceFaceID.Storage.Repository;

public class AttendanceRepository(AttendanceContext ef)
{
    public async Task<IList<Attendance>> GetAttendanceByDayFromStudent(long studentId, DateTime date)
    {
        return await ef.Attendances.Where(x => x.StudentId == studentId)
            .Where(x => x.DateTimeJoined.Date == date).ToListAsync();
    }
    
    public async Task AddAttendance(Attendance attendance)
    {
        await ef.Attendances.AddAsync(attendance);
        await ef.SaveChangesAsync();
    }

    public async Task UpdateAttendance(Attendance attendance)
    {
        ef.Update(attendance);
        await ef.SaveChangesAsync();
    }

    public async Task RemoveAttendance(Attendance attendance)
    {
        ef.Remove(attendance);
        await ef.SaveChangesAsync();
    }

    public async Task<bool> ExistsAttendance(long attendanceStudentId, DateTime dateTime)
    {
       return await ef.Attendances.AnyAsync(x=> x.StudentId == attendanceStudentId && x.DateTimeJoined == dateTime);
    }
    
    public async Task<bool> ExistsAttendanceOfDate(long attendanceStudentId, DateTime dateTime)
    {
        return await ef.Attendances.AnyAsync(x => x.StudentId == attendanceStudentId 
                                                  && x.DateTimeJoined.Date == dateTime.Date);
    }

    public async Task<IList<Attendance>> GetHistoryFromDateByStudentId(long studentId, DateTime dateTime)
    {
        return await ef.Attendances.Where(x => x.StudentId == studentId 
                                                  && x.DateTimeJoined.Date == dateTime.Date).ToListAsync();
    }
}