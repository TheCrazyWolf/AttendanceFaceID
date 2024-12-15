using AttendanceFaceID.Models.DLA;
using Microsoft.EntityFrameworkCore;

namespace AttendanceFaceID.Storage.Context;

public sealed class AttendanceContext : DbContext
{
    //public AttendanceContext() => Database.MigrateAsync();
    public DbSet<Group> Groups { get; set; }
    public DbSet<UnificationProfile> UnificationProfiles { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Station> Stations { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = app.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany()
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}