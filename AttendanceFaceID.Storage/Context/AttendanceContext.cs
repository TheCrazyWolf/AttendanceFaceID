using AttendanceFaceID.Models.DLA;
using Microsoft.EntityFrameworkCore;

namespace AttendanceFaceID.Storage.Context;

public sealed class AttendanceContext : DbContext
{
    public AttendanceContext() => Database.MigrateAsync();
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = app.db");
    }
}