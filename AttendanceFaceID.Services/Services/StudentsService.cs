using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Services.Models;
using AttendanceFaceID.Storage;

namespace AttendanceFaceID.Services.Services;

public class StudentsService(AttendanceMainRepo repository)
{
    public async Task<ActionResult> ImportStudent(Student student, string groupName)
    {
        groupName = groupName.ToUpper().Replace(" ", string.Empty);
        student.FirstName = student.FirstName.ToUpper().Replace(" ", string.Empty);
        student.LastName = student.LastName.ToUpper().Replace(" ", string.Empty);
        student.MiddleName = student.MiddleName.ToUpper().Replace(" ", string.Empty);
        student.ShortName = student.GetShortName();
        
        var groupEntity = await repository.Groups.GetGroupByNameAsync(groupName.ToUpper());
        
        if (groupEntity is null)
        {
            groupEntity = new Group() { Name = groupName };
            await repository.Groups.CreateGroupAsync(groupEntity);
        }
        var studentIdentity = await repository.Students.GetStudentByShortName(student.ShortName);

        if (studentIdentity is not null) return new ActionResult(false, $"Студент {student.ShortName} уже существует");
        student.GroupId = groupEntity.Id;
        await repository.Students.AddStudent(student);
        return new ActionResult(true, $"Студент {student.ShortName} импортирован");
    }

    public async Task<IList<Student>> GetStudentByGroup(long groupdId)
    {
        return await repository.Students.GetStudentsFromGroup(groupdId);
    }

    public async Task<Student?> GetStudentById(long studentId)
    {
        return await repository.Students.GetStudentById(studentId);
    }

    public async Task RemoveStudent(Student student)
    {
        await repository.Students.RemoveStudent(student);
    }
}