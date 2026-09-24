using CourseScheduleSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }


public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<ClassRepresentative> ClassRepresentatives => Set<ClassRepresentative>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Lecturer> Lecturers => Set<Lecturer>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<ScheduleEntry> ScheduleEntries => Set<ScheduleEntry>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<ClassGroup> ClassGroups => Set<ClassGroup>();
    public DbSet<CourseCompletion> CourseCompletions => Set<CourseCompletion>();
    public DbSet<HodMessage> HodMessages => Set<HodMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureAdministrator(modelBuilder);
        ConfigureClassRepresentative(modelBuilder);
        ConfigureStudent(modelBuilder);
        ConfigureLecturer(modelBuilder);
        ConfigureCourse(modelBuilder);
        ConfigureRoom(modelBuilder);
        ConfigureScheduleEntry(modelBuilder);
        ConfigureEnrollment(modelBuilder);
        ConfigureExam(modelBuilder);
        ConfigureSupportTicket(modelBuilder);
        ConfigureClassGroup(modelBuilder);
        ConfigureCourseCompletion(modelBuilder);
        ConfigureHodMessage(modelBuilder);
    }

    private static void ConfigureAdministrator(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasIndex(a => a.StaffId)
                .IsUnique();

            entity.Property(a => a.StaffId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(a => a.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.Department)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Role)
                .IsRequired()
                .HasMaxLength(50);
        });
    }

    private static void ConfigureClassRepresentative(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClassRepresentative>(entity =>
        {
            entity.HasKey(cp => cp.Id);

            entity.HasIndex(cp => cp.RegNo)
                .IsUnique();

            entity.HasIndex(cp => new
            {
                cp.Department,
                cp.Intake,
                cp.Level,
                cp.IsActive
            });

            entity.Property(cp => cp.RegNo)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(cp => cp.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(cp => cp.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(cp => cp.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(cp => cp.Nationality)
                .HasMaxLength(100);

            entity.Property(cp => cp.Faculty)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(cp => cp.Department)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(cp => cp.Year)
                .IsRequired();

            entity.Property(cp => cp.StudySession)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(cp => cp.IsActive)
                .IsRequired();

            entity.Property(cp => cp.PasswordHash)
                .IsRequired();

            entity.Property(cp => cp.AssignedAt)
                .IsRequired();

            entity.Property(cp => cp.SignatureImageData)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(cp => cp.Intake)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(cp => cp.Level)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasMany(cp => cp.RepresentedCourses)
                .WithMany(course => course.ClassRepresentatives)
                .UsingEntity(join =>
                    join.ToTable("CourseClassRepresentatives"));

            entity.HasMany(cp => cp.ClassGroups)
                .WithOne(group => group.ClassRepresentative)
                .HasForeignKey(group => group.ClassRepresentativeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(cp => cp.CourseCompletions)
                .WithOne(completion => completion.ClassRepresentative)
                .HasForeignKey(completion => completion.ClassRepresentativeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(cp => cp.HodMessages)
                .WithOne(message => message.ClassRepresentative)
                .HasForeignKey(message => message.ClassRepresentativeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(cp => cp.ScheduleEntries)
                .WithOne(schedule => schedule.ClassRepresentative)
                .HasForeignKey(schedule => schedule.ClassRepresentativeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureStudent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.HasIndex(s => s.RegNo)
                .IsUnique();

            entity.Property(s => s.RegNo)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(s => s.Department)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Program)
                .HasMaxLength(50);

            entity.Property(s => s.Level)
                .HasMaxLength(50);
        });
    }

    private static void ConfigureLecturer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.HasIndex(l => l.StaffId)
                .IsUnique();

            entity.HasIndex(l => new
            {
                l.Department,
                l.IsActive
            });

            entity.Property(l => l.StaffId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(l => l.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(l => l.Email)
                .HasMaxLength(256);

            entity.Property(l => l.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(l => l.Department)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(l => l.PasswordHash)
                .IsRequired();

            entity.Property(l => l.IsActive)
                .IsRequired();

            entity.HasMany(l => l.Courses)
                .WithOne(c => c.Lecturer)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(l => l.ScheduleEntries)
                .WithOne(s => s.Lecturer)
                .HasForeignKey(s => s.LecturerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany<CourseCompletion>()
                .WithOne(completion => completion.Lecturer)
                .HasForeignKey(completion => completion.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCourse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasIndex(c => c.Code)
                .IsUnique();

            entity.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Description)
                .HasMaxLength(1000);

            entity.HasOne(c => c.Administrator)
                .WithMany(a => a.ManagedCourses)
                .HasForeignKey(c => c.AdministratorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(c => c.ScheduleEntries)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<Exam>()
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<ClassGroup>()
                .WithOne(group => group.Course)
                .HasForeignKey(group => group.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany<CourseCompletion>()
                .WithOne(completion => completion.Course)
                .HasForeignKey(completion => completion.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRoom(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasIndex(r => new
            {
                r.Building,
                r.RoomNumber
            })
            .IsUnique();

            entity.HasIndex(r => new
            {
                r.IsAvailable,
                r.Building
            });

            entity.Property(r => r.RoomNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(r => r.Building)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(r => r.Capacity)
                .IsRequired();

            entity.Property(r => r.RoomType)
                .HasMaxLength(100);

            entity.Property(r => r.IsAvailable)
                .IsRequired();
        });
    }

    private static void ConfigureScheduleEntry(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleEntry>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.HasIndex(s => new
            {
                s.RoomId,
                s.DayOfWeek,
                s.StartTime,
                s.EndTime,
                s.IsActive
            });

            entity.HasIndex(s => new
            {
                s.LecturerId,
                s.DayOfWeek,
                s.StartTime,
                s.EndTime,
                s.IsActive
            });

            entity.HasIndex(s => new
            {
                s.ClassRepresentativeId,
                s.IsActive
            });

            entity.HasIndex(s => new
            {
                s.CourseId,
                s.IsActive
            });

            entity.HasOne(s => s.Room)
                .WithMany(r => r.ScheduleEntries)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Lecturer)
                .WithMany(l => l.ScheduleEntries)
                .HasForeignKey(s => s.LecturerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(s => s.ClassRepresentative)
                .WithMany(cp => cp.ScheduleEntries)
                .HasForeignKey(s => s.ClassRepresentativeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(s => s.StartDate)
                .IsRequired()
                .HasColumnType("date");

            entity.Property(s => s.EndDate)
                .IsRequired()
                .HasColumnType("date");

            entity.Property(s => s.StudySession)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(s => s.Notes)
                .HasMaxLength(500);

            entity.Property(s => s.IsActive)
                .IsRequired();

            entity.Property(s => s.CreatedAt)
                .IsRequired();

            entity.Property(s => s.UpdatedAt);
        });
    }

    private static void ConfigureEnrollment(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new
            {
                e.StudentId,
                e.CourseId
            })
            .IsUnique();

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureExam(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Room)
                .WithMany()
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);
        });
    }

    private static void ConfigureSupportTicket(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.HasIndex(t => t.TicketNumber)
                .IsUnique();

            entity.Property(t => t.TicketNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureClassGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClassGroup>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.GroupName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.Intake)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.Level)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(g => g.GroupLink)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(g => g.CreatedAt)
                .IsRequired();

            entity.HasIndex(g => new
            {
                g.CourseId,
                g.ClassRepresentativeId,
                g.Intake,
                g.Level
            })
            .IsUnique();
        });
    }

    private static void ConfigureCourseCompletion(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourseCompletion>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Status)
                .IsRequired();

            entity.Property(c => c.Remarks)
                .HasMaxLength(1000);

            entity.HasIndex(c => new
            {
                c.CourseId,
                c.ClassRepresentativeId
            })
            .IsUnique();

            entity.Property(c => c.SubmittedAt);

            entity.Property(c => c.ConfirmedAt);
        });
    }

    private static void ConfigureHodMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HodMessage>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Message)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(m => m.SentAt)
                .IsRequired();

            entity.Property(m => m.IsRead)
                .IsRequired();
        });
    }


}
