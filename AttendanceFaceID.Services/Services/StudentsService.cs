using System.Collections;
using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Services.Models;
using AttendanceFaceID.Services.Models.xlsx;
using AttendanceFaceID.Storage;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Forms;

namespace AttendanceFaceID.Services.Services;

public class StudentsService(AttendanceMainRepo repository)
{
    public async Task<IList<Student>> GetStudentByGroup(long groupId)
    {
        return await repository.Students.GetStudentsFromGroup(groupId);
    }

    public async Task<Student?> GetStudentById(long studentId)
    {
        return await repository.Students.GetStudentById(studentId);
    }

    public async Task RemoveStudent(Student student)
    {
        await repository.Students.RemoveStudent(student);
    }

    public async Task<IList<IXLRow>> ReadXlsxFileAndGetRows(IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        
        var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);

        return worksheet.RowsUsed().Skip(1).ToList();
    }

    public RowStudent? TryGetRowFromCell(List<IXLCell> cells)
    {
        try
        {
            string lastName = cells[0].Value.ToString();
            string firstName = cells[1].Value.ToString();
            string middleName = cells[2].Value.ToString();
            string groupName = cells[3].Value.ToString();
            return new RowStudent(lastName.ToUpper().Trim(), firstName.ToUpper().Trim(),
                middleName.ToUpper().Trim(), groupName.ToUpper().Trim());
        }
        catch
        {
            return null;
        }
    }

    public async Task<ActionResult> ImportStudent(RowStudent rowEventSkud)
    {
        Student student = new Student()
        {
            ShortName = rowEventSkud.GetShortName()
        };
        
        var groupEntity = await repository.Groups.GetGroupByNameAsync(rowEventSkud.Group);
        
        if (groupEntity is null)
        {
            groupEntity = new Group() { Name = rowEventSkud.Group };
            await repository.Groups.CreateGroupAsync(groupEntity);
        }
        var studentIdentity = await repository.Students.GetStudentByShortNameAndGroupId(student.ShortName, groupEntity.Id);
        if (studentIdentity is not null) return new ActionResult(false, $"Студент {student.ShortName} уже существует");
        student.GroupId = groupEntity.Id;
        await repository.Students.AddStudent(student);
        return new ActionResult(true, $"Студент {student.ShortName} импортирован");
    }
}