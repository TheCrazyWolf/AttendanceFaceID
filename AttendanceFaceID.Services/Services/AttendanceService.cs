using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Models.Enums;
using AttendanceFaceID.Services.Models;
using AttendanceFaceID.Services.Models.xlsx;
using AttendanceFaceID.Storage;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Forms;

namespace AttendanceFaceID.Services.Services;

public class AttendanceService(AttendanceMainRepo repository)
{
    public async Task<ActionResult> ImportAttendance(string dateTime, string objectInit,
        string shortName, string groupName, string faceMode)
    {
        groupName = groupName.Replace(" ", string.Empty).Replace("  ", string.Empty).ToUpper();
        var groupEntity = await repository.Groups.GetGroupByNameAsync(groupName);
        
        if (groupEntity is null)
        {
            groupEntity = new Group() { Name = groupName };
            await repository.Groups.CreateGroupAsync(groupEntity);
        }
        
        shortName = shortName.Replace($"({groupName})", string.Empty).ToUpper().Trim();
        
        if (!DateTime.TryParse(dateTime, out DateTime dateTimeValue))
        {
            return new ActionResult(false, $"{dateTime} - {shortName}. Ошибка парсинга, не удалось обработать дату захода");
        }
        
        var studentIdentity = await repository.Students.GetStudentByShortName(shortName, groupEntity.Id);

        if (studentIdentity is null)
        {
            studentIdentity = new Student() { LastName = "ImportByFaceId", FirstName = "ImportByFaceId", MiddleName = "ImportByFaceId"
                ,ShortName = shortName, GroupId = groupEntity.Id };

            await repository.Students.AddStudent(studentIdentity);
        }

        AttendanceEnum type = faceMode switch
        {
            "7" => AttendanceEnum.Face,
            "4" => AttendanceEnum.Card,
            _ => AttendanceEnum.Unknow
        };

        Attendance attendance = new Attendance()
        {
            StudentId = studentIdentity.Id,
            ObjectInizialized = objectInit,
            DateTimeJoined = dateTimeValue,
            ModeType = type
        };
        
        if(attendance.ObjectInizialized.Contains("Общежитие", StringComparison.InvariantCultureIgnoreCase))
            return new ActionResult(false, $"Событие произошло в общежитиет: {attendance.DateTimeJoined}.{studentIdentity.ShortName} гр. {groupEntity.Name}");
        
        if(await repository.Attendances.ExistsAttendance(attendance.StudentId ?? 0, attendance.DateTimeJoined))
            return new ActionResult(false, $"Событие уже существует: {attendance.DateTimeJoined}.{studentIdentity.ShortName} гр. {groupEntity.Name}");
        
        await repository.Attendances.AddAttendance(attendance);
        return new ActionResult(true, $"Событие импортировано: {attendance.DateTimeJoined}.{studentIdentity.ShortName} гр. {groupEntity.Name}");
    }

    public async Task<bool> ExistsAttendanceOfDate(Student student, DateTime dateTime)
    {
        return await repository.Attendances.ExistsAttendanceOfDate(student.Id, dateTime);
    }


    public async Task<IList<Attendance>> GetHistoryFromDateByStudentId(long studentId, DateTime dateTime)
    {
        return await repository.Attendances.GetHistoryFromDateByStudentId(studentId, dateTime);
    }

    public async Task<Attendance?> GetLastAttendanceFromStudent(long studentId)
    {
        return await repository.Attendances.GetLastAttendanceFromStudent(studentId);
    }

    public async Task<IList<IXLRow>> ReadXlsxFileAndGetRows(IBrowserFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        
        var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);

        return worksheet.RowsUsed().Skip(1).ToList();
    }

    public RowEventSkud? TryGetRowEventSkud(List<IXLCell>? cells)
    {
        if(cells is null) return null;

        try
        {
            string dateTime = cells[7].Value.ToString();
            string objectInit = cells[4].Value.ToString();
            string shortName = cells[8].Value.ToString();
            string groupName = cells[6].Value.ToString();
            string faceMode = cells[9].Value.ToString();
            return new RowEventSkud(dateTime, objectInit, shortName, groupName, faceMode);
        }
        catch 
        {
            return null;
        }
    }

    public async Task<ActionResult> ImportAttendance(RowEventSkud rowEventSkud)
    {
        return await ImportAttendance(dateTime: rowEventSkud.DateTime, objectInit: rowEventSkud.ObjectInit,
            shortName: rowEventSkud.ShortName,
            groupName: rowEventSkud.GroupName, faceMode: rowEventSkud.FaceMode);
    }
}