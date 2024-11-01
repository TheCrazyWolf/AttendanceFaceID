using AttendanceFaceID.Models.DLA;
using AttendanceFaceID.Models.Enums;
using AttendanceFaceID.Services.Enums;
using AttendanceFaceID.Services.Models;
using AttendanceFaceID.Services.Models.Attendances;
using AttendanceFaceID.Services.Models.xlsx;
using AttendanceFaceID.Storage;
using ClientSamgk;
using ClientSamgkOutputResponse.Interfaces.Schedule;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Forms;

namespace AttendanceFaceID.Services.Services;

public class AttendanceService(AttendanceMainRepo repository, ClientSamgkApi clientSamgkApi)
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
            return new ActionResult(false,
                $"{dateTime} - {shortName}. Ошибка парсинга, не удалось обработать дату захода");
        }

        var studentIdentity = await repository.Students.GetStudentByShortName(shortName, groupEntity.Id);

        if (studentIdentity is null)
        {
            studentIdentity = new Student()
            {
                LastName = "ImportByFaceId", FirstName = "ImportByFaceId", MiddleName = "ImportByFaceId",
                ShortName = shortName, GroupId = groupEntity.Id
            };

            await repository.Students.AddStudent(studentIdentity);
        }
        
        var station = await repository.Stations.GetStationByName(objectInit);
        
        if (station is null)
        {
            station = new Station()
            {
                Name = objectInit
            };

            await repository.Stations.AddStation(station);
        }

        AttendanceEnum type = faceMode switch
        {
            "7" => AttendanceEnum.Face,
            "4" => AttendanceEnum.Card,
            _ => AttendanceEnum.Unknow
        };

        Attendance attendance = new Attendance()
        {
            StudentId = station.Id,
            StationId = station.Id,
            DateTimeJoined = dateTimeValue,
            ModeType = type
        };

        if (station.Name.Contains("Общежитие", StringComparison.InvariantCultureIgnoreCase))
            return new ActionResult(false,
                $"Событие произошло в общежитиет: {attendance.DateTimeJoined}.{station.Name} гр. {groupEntity.Name}");

        if (await repository.Attendances.ExistsAttendance(attendance.StudentId ?? 0, attendance.DateTimeJoined))
            return new ActionResult(false,
                $"Событие уже существует: {attendance.DateTimeJoined}.{station.Name} гр. {groupEntity.Name}");

        await repository.Attendances.AddAttendance(attendance);
        return new ActionResult(true,
            $"Событие импортировано: {attendance.DateTimeJoined}.{station.Name} гр. {groupEntity.Name}");
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
        await file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);

        return worksheet.RowsUsed().Skip(1).ToList();
    }

    public RowEventSkud? TryGetRowEventSkud(List<IXLCell>? cells)
    {
        if (cells is null) return null;

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

    public async Task<IList<AttendanceStudentDetails>> GetAttendanceResultFromDate(IList<Student> students,
        Group selectedGroup,
        DateTime dateTimeStart, DateTime dateTimeEnd, bool skipWeekend, bool useScheduleIntegration)
    {
        var result = new List<AttendanceStudentDetails>();

        IList<IResultOutScheduleFromDate> scheduleFromDates = new List<IResultOutScheduleFromDate>();
        
        foreach (var student in students)
        {
            var studentDetails = new AttendanceStudentDetails(student.Id, student.ShortName);
            var currentDate = dateTimeStart;

            while (currentDate <= dateTimeEnd)
            {
                AttendanceResultDayDetails attendanceResultDayDetails = new AttendanceResultDayDetails();
                attendanceResultDayDetails.Date = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);

                if (currentDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday && skipWeekend)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                AttendanceEnumType? typeOfDay = null;

                var groupEntity = await clientSamgkApi.Groups.GetGroupAsync(selectedGroup.Name);
                
                if (useScheduleIntegration && groupEntity != null)
                {
                    IResultOutScheduleFromDate currentDateLesson =
                        scheduleFromDates.FirstOrDefault(x => x.Date.Day == currentDate.Day && x.Date.Month == currentDate.Month && x.Date.Year == currentDate.Year) ??
                        await clientSamgkApi.Schedule.GetScheduleAsync(DateOnly.FromDateTime(currentDate), groupEntity);

                    scheduleFromDates.Add(currentDateLesson);
                    
                    if (currentDateLesson.Lessons.Count is 0) typeOfDay = AttendanceEnumType.NoSchedule;
                    
                    var firstLesson = currentDateLesson.Lessons.FirstOrDefault();
                    var firstCab = currentDateLesson.Lessons.FirstOrDefault()?.Cabs.FirstOrDefault();

                    if (firstLesson is not null && firstCab is not null)
                    {
                        typeOfDay = firstCab.Auditory switch
                        {
                            "дист" => AttendanceEnumType.Distance,
                            "п" => AttendanceEnumType.Practice,
                            _ => null
                        };
                    }

                    foreach (var item in currentDateLesson.Lessons)
                    {
                        if (item.NumLesson is 0)
                            attendanceResultDayDetails.TotalHoursLessons += 2;
                        else
                            attendanceResultDayDetails.TotalHoursLessons += 1;
                    }
                }

                if (typeOfDay is not null)
                    attendanceResultDayDetails.AttendanceType = typeOfDay.Value;
                else if (typeOfDay is AttendanceEnumType.NoSchedule)
                    attendanceResultDayDetails.AttendanceType = typeOfDay.Value;
                else 
                    attendanceResultDayDetails.AttendanceType = await ExistsAttendanceOfDate(student, currentDate)
                        ? AttendanceEnumType.Came
                        : AttendanceEnumType.Absent;

                studentDetails.Attendances.Add(attendanceResultDayDetails);
                
                currentDate = currentDate.Date.AddDays(1);
            }
            
            result.Add(studentDetails);
        }

        return result;
    }

    public async Task<Attendance?> GetLastAttendanceFromDb()
    {
        return await repository.Attendances.GetLastAttendanceFromDb();
    }
}