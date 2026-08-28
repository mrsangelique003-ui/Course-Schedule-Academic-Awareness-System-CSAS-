using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── Core academic tables ─────────────────────────────────────────────────
    public DbSet<Lecturer>           Lecturers           { get; set; }
    public DbSet<Course>             Courses             { get; set; }
    public DbSet<Room>               Rooms               { get; set; }
    public DbSet<ScheduleEntry>      ScheduleEntries     { get; set; }
    public DbSet<RoomShift>          RoomShifts          { get; set; }
    public DbSet<Enrollment>         Enrollments         { get; set; }
    public DbSet<ClassRepresentative> ClassRepresentatives { get; set; }
    public DbSet<LecturerAttendance> LecturerAttendances  { get; set; }
    public DbSet<AttendanceFlag>     AttendanceFlags      { get; set; }
    public DbSet<Notification>       Notifications        { get; set; }
    public DbSet<AuditLog>           AuditLogs            { get; set; }

    // ── Auth helpers ─────────────────────────────────────────────────────────
    public DbSet<RefreshToken>       RefreshTokens        { get; set; }
    public DbSet<OtpCode>            OtpCodes             { get; set; }

    // ── Services / Applications / Documents ─────────────────────────────────
    public DbSet<Service>            Services             { get; set; }
    public DbSet<Application>        Applications         { get; set; }
    public DbSet<Document>           Documents            { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ── ApplicationUser extra columns ─────────────────────────────────
        b.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.RegNo).HasMaxLength(50);
            e.Property(u => u.FullName).HasMaxLength(200);
            e.Property(u => u.Department).HasMaxLength(100).HasDefaultValue("CIS");
            e.Property(u => u.StudySession).HasMaxLength(20).HasDefaultValue("Day");
            e.HasIndex(u => u.RegNo).IsUnique();
        });

        // ── Lecturer ─────────────────────────────────────────────────────
        b.Entity<Lecturer>(e =>
        {
            e.HasIndex(l => l.StaffId).IsUnique();
            e.Property(l => l.StaffId).HasMaxLength(50);
            e.Property(l => l.Department).HasMaxLength(100);
        });

        // ── Course ────────────────────────────────────────────────────────
        b.Entity<Course>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.Property(c => c.Code).HasMaxLength(20);
            e.Property(c => c.Status).HasMaxLength(30).HasDefaultValue("Available");

            e.HasOne(c => c.Lecturer)
             .WithMany(l => l.Courses)
             .HasForeignKey(c => c.LecturerId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Room ──────────────────────────────────────────────────────────
        b.Entity<Room>(e =>
        {
            e.HasIndex(r => r.RoomNumber).IsUnique();
            e.Property(r => r.RoomNumber).HasMaxLength(30);
            e.Property(r => r.Building).HasMaxLength(100);
        });

        // ── ScheduleEntry ─────────────────────────────────────────────────
        b.Entity<ScheduleEntry>(e =>
        {
            e.HasOne(s => s.Course)
             .WithMany(c => c.ScheduleEntries)
             .HasForeignKey(s => s.CourseId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(s => s.Room)
             .WithMany(r => r.ScheduleEntries)
             .HasForeignKey(s => s.RoomId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(s => s.DayOfWeek).HasMaxLength(10);
            e.Property(s => s.StudySession).HasMaxLength(20).HasDefaultValue("Day");
        });

        // ── RoomShift ─────────────────────────────────────────────────────
        b.Entity<RoomShift>(e =>
        {
            e.HasOne(rs => rs.ScheduleEntry)
             .WithMany(se => se.RoomShifts)
             .HasForeignKey(rs => rs.ScheduleEntryId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(rs => rs.OriginalRoom)
             .WithMany(r => r.OriginalShifts)
             .HasForeignKey(rs => rs.OriginalRoomId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(rs => rs.NewRoom)
             .WithMany(r => r.NewShifts)
             .HasForeignKey(rs => rs.NewRoomId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(rs => rs.CreatedBy)
             .WithMany(u => u.RoomShifts)
             .HasForeignKey(rs => rs.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(rs => rs.Status).HasMaxLength(20).HasDefaultValue("Pending");
        });

        // ── Enrollment ────────────────────────────────────────────────────
        b.Entity<Enrollment>(e =>
        {
            e.HasIndex(en => new { en.UserId, en.CourseId }).IsUnique();
            e.Property(en => en.Status).HasMaxLength(20).HasDefaultValue("Pending");

            e.HasOne(en => en.User)
             .WithMany(u => u.Enrollments)
             .HasForeignKey(en => en.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(en => en.Course)
             .WithMany(c => c.Enrollments)
             .HasForeignKey(en => en.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ClassRepresentative ───────────────────────────────────────────
        b.Entity<ClassRepresentative>(e =>
        {
            e.HasOne(cr => cr.User)
             .WithMany(u => u.ClassRepresentatives)
             .HasForeignKey(cr => cr.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(cr => cr.Course)
             .WithMany(c => c.ClassRepresentatives)
             .HasForeignKey(cr => cr.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── LecturerAttendance ────────────────────────────────────────────
        b.Entity<LecturerAttendance>(e =>
        {
            e.HasOne(la => la.Lecturer)
             .WithMany(l => l.LecturerAttendances)
             .HasForeignKey(la => la.LecturerId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(la => la.ScheduleEntry)
             .WithMany(se => se.LecturerAttendances)
             .HasForeignKey(la => la.ScheduleEntryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(la => la.AttendanceStatus).HasMaxLength(20).HasDefaultValue("Present");
        });

        // ── AttendanceFlag ────────────────────────────────────────────────
        b.Entity<AttendanceFlag>(e =>
        {
            e.HasOne(af => af.RaisedBy)
             .WithMany(u => u.AttendanceFlags)
             .HasForeignKey(af => af.RaisedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(af => af.ScheduleEntry)
             .WithMany(se => se.AttendanceFlags)
             .HasForeignKey(af => af.ScheduleEntryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(af => af.IssueType).HasMaxLength(20).HasDefaultValue("Absent");
            e.Property(af => af.Status).HasMaxLength(20).HasDefaultValue("Pending");
        });

        // ── Notification ──────────────────────────────────────────────────
        b.Entity<Notification>(e =>
        {
            e.HasOne(n => n.User)
             .WithMany(u => u.Notifications)
             .HasForeignKey(n => n.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(n => n.Type).HasMaxLength(30).HasDefaultValue("System");
        });

        // ── AuditLog ──────────────────────────────────────────────────────
        b.Entity<AuditLog>(e =>
        {
            e.HasOne(al => al.User)
             .WithMany(u => u.AuditLogs)
             .HasForeignKey(al => al.UserId)
             .OnDelete(DeleteBehavior.SetNull);

            e.Property(al => al.OldValues).HasColumnType("nvarchar(max)");
            e.Property(al => al.NewValues).HasColumnType("nvarchar(max)");
        });

        // ── RefreshToken ──────────────────────────────────────────────────
        b.Entity<RefreshToken>(e =>
        {
            e.HasIndex(rt => rt.Token).IsUnique();
            e.HasOne(rt => rt.User)
             .WithMany()
             .HasForeignKey(rt => rt.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── OtpCode ───────────────────────────────────────────────────────
        b.Entity<OtpCode>(e =>
        {
            e.HasOne(o => o.User)
             .WithMany()
             .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(o => o.Purpose).HasMaxLength(50).HasDefaultValue("Login");
        });

        // ── Service ───────────────────────────────────────────────────────
        b.Entity<Service>(e =>
        {
            e.HasIndex(s => s.Code).IsUnique();
            e.Property(s => s.Status).HasMaxLength(20).HasDefaultValue("Active");
            e.Property(s => s.Fee).HasColumnType("decimal(10,2)");
        });

        // ── Application ───────────────────────────────────────────────────
        b.Entity<Application>(e =>
        {
            e.HasIndex(a => a.ReferenceNumber).IsUnique();
            e.Property(a => a.Status).HasMaxLength(30).HasDefaultValue("Draft");
            e.Property(a => a.Priority).HasMaxLength(20).HasDefaultValue("Normal");

            e.HasOne(a => a.Service)
             .WithMany(s => s.Applications)
             .HasForeignKey(a => a.ServiceId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Applicant)
             .WithMany(u => u.Applications)
             .HasForeignKey(a => a.ApplicantUserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.ReviewedBy)
             .WithMany()
             .HasForeignKey(a => a.ReviewedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Document ──────────────────────────────────────────────────────
        b.Entity<Document>(e =>
        {
            e.HasOne(d => d.Application)
             .WithMany(a => a.Documents)
             .HasForeignKey(d => d.ApplicationId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.UploadedBy)
             .WithMany()
             .HasForeignKey(d => d.UploadedByUserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(d => d.Status).HasMaxLength(20).HasDefaultValue("Pending");
        });
    }
}
