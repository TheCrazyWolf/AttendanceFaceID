using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Models.Enums;
using AttendanceFaceID.Services.Models;
using AttendanceFaceID.Storage;

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
        
        if(await repository.AttendancesMain.ExistsAttendance(attendance.StudentId ?? 0, attendance.DateTimeJoined))
            return new ActionResult(false, $"Событие уже существует: {attendance.DateTimeJoined}.{studentIdentity.ShortName} гр. {groupEntity.Name}");
        
        await repository.AttendancesMain.AddAttendance(attendance);
        return new ActionResult(true, $"Событие импортировано: {attendance.DateTimeJoined}.{studentIdentity.ShortName} гр. {groupEntity.Name}");
    }
}