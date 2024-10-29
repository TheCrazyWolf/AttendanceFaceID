﻿using AttendanceFaceID.Storage.Context;
using AttendanceFaceID.Storage.Repository;

namespace AttendanceFaceID.Storage;

public class AttendanceMainRepo(AttendanceContext ef)
{
    public AttendanceRepository AttendancesMain { get; set; } = new AttendanceRepository(ef);
    public GroupsRepository Groups { get; set; } = new GroupsRepository(ef);
    public StudentsRepository Students { get; set; } = new StudentsRepository(ef);
}