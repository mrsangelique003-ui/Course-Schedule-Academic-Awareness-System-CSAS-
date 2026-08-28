using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db          = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        // Apply any pending migrations automatically
        await db.Database.MigrateAsync();

        // ── 1. Seed roles ──────────────────────────────────────────────────
        string[] roles = ["Student", "CP", "DirectorOfQuality", "Dean", "HOD"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole(role));
        }

        // ── 2. Seed demo users ─────────────────────────────────────────────
        await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName     = "STU001",
            RegNo        = "STU001",
            FullName     = "Alice Uwimana",
            Email        = "alice@unilak.ac.rw",
            Department   = "CIS",
            StudySession = "Day",
            IsActive     = true
        }, "Pass@123", "Student");

        await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName     = "CP001",
            RegNo        = "CP001",
            FullName     = "Bob Nkurunziza",
            Email        = "bob@unilak.ac.rw",
            Department   = "CIS",
            StudySession = "Day",
            IsActive     = true
        }, "Pass@123", "CP");

        await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName     = "DIR001",
            RegNo        = "DIR001",
            FullName     = "Dr. Claire Mukamana",
            Email        = "claire@unilak.ac.rw",
            Department   = "CIS",
            StudySession = "Day",
            IsActive     = true
        }, "Pass@123", "DirectorOfQuality");

        await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName     = "DEAN001",
            RegNo        = "DEAN001",
            FullName     = "Prof. Denis Habimana",
            Email        = "denis@unilak.ac.rw",
            Department   = "CIT",
            StudySession = "Day",
            IsActive     = true
        }, "Pass@123", "Dean");

        await SeedUserAsync(userManager, new ApplicationUser
        {
            UserName     = "HOD001",
            RegNo        = "HOD001",
            FullName     = "Prof. Eric Mugisha",
            Email        = "eric@unilak.ac.rw",
            Department   = "CIS",
            StudySession = "Day",
            IsActive     = true
        }, "Pass@123", "HOD");

        // ── 3. Seed Lecturers ──────────────────────────────────────────────
        if (!await db.Lecturers.AnyAsync())
        {
            db.Lecturers.AddRange(
                new Lecturer { StaffId = "LEC001", FullName = "Dr. Jean Mugisha",       Email = "j.mugisha@unilak.ac.rw",    Department = "CIS" },
                new Lecturer { StaffId = "LEC002", FullName = "Mrs. Anne Byukusenge",   Email = "a.byukusenge@unilak.ac.rw", Department = "CIS" },
                new Lecturer { StaffId = "LEC003", FullName = "Mr. Emmanuel Niyonsenga",Email = "e.niyonsenga@unilak.ac.rw", Department = "CIS" },
                new Lecturer { StaffId = "LEC004", FullName = "Dr. Patrick Habimana",   Email = "p.habimana@unilak.ac.rw",   Department = "CIS" }
            );
            await db.SaveChangesAsync();
        }

        // ── 4. Seed Rooms ──────────────────────────────────────────────────
        if (!await db.Rooms.AnyAsync())
        {
            db.Rooms.AddRange(
                new Room { RoomNumber = "A204", Building = "Block A", Capacity = 60 },
                new Room { RoomNumber = "B101", Building = "Block B", Capacity = 80 },
                new Room { RoomNumber = "A108", Building = "Block A", Capacity = 40 },
                new Room { RoomNumber = "C302", Building = "Block C", Capacity = 50 },
                new Room { RoomNumber = "LAB01", Building = "Block D", Capacity = 30 }
            );
            await db.SaveChangesAsync();
        }

        // ── 5. Seed Courses ────────────────────────────────────────────────
        if (!await db.Courses.AnyAsync())
        {
            var lecturers = await db.Lecturers.ToListAsync();
            db.Courses.AddRange(
                new Course
                {
                    Code = "CSE301", Name = "Software Engineering",
                    Description = "Software lifecycle, design patterns and best practices.",
                    Status = "Available", LecturerId = lecturers[0].Id,
                    WhatsAppGroupLink = "https://chat.whatsapp.com/example1",
                    StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 12, 15),
                    RegistrationOpenDate = new DateTime(2026, 8, 25), RegistrationCloseDate = new DateTime(2026, 9, 4)
                },
                new Course
                {
                    Code = "CSE305", Name = "Database Systems",
                    Description = "Relational databases, SQL, and database design.",
                    Status = "Available", LecturerId = lecturers[1].Id,
                    WhatsAppGroupLink = "https://chat.whatsapp.com/example2",
                    StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 12, 15),
                    RegistrationOpenDate = new DateTime(2026, 8, 25), RegistrationCloseDate = new DateTime(2026, 9, 4)
                },
                new Course
                {
                    Code = "CSE310", Name = "Human-Computer Interaction",
                    Description = "UX design, accessibility and usability principles.",
                    Status = "Available", LecturerId = lecturers[2].Id,
                    WhatsAppGroupLink = "https://chat.whatsapp.com/example3",
                    StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 12, 15),
                    RegistrationOpenDate = new DateTime(2026, 8, 25), RegistrationCloseDate = new DateTime(2026, 9, 4)
                },
                new Course
                {
                    Code = "CSE402", Name = "Software Architecture",
                    Description = "Architectural patterns, microservices, and system design.",
                    Status = "Available", LecturerId = lecturers[3].Id,
                    WhatsAppGroupLink = "https://chat.whatsapp.com/example4",
                    StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 12, 15),
                    RegistrationOpenDate = new DateTime(2026, 8, 25), RegistrationCloseDate = new DateTime(2026, 9, 4)
                }
            );
            await db.SaveChangesAsync();
        }

        // ── 6. Seed Schedule Entries ───────────────────────────────────────
        if (!await db.ScheduleEntries.AnyAsync())
        {
            var courses = await db.Courses.ToListAsync();
            var rooms   = await db.Rooms.ToListAsync();
            db.ScheduleEntries.AddRange(
                new ScheduleEntry { CourseId = courses[0].Id, DayOfWeek = "Monday",    StartTime = new TimeOnly(8,0),  EndTime = new TimeOnly(10,0), StudySession = "Day", RoomId = rooms[0].Id },
                new ScheduleEntry { CourseId = courses[1].Id, DayOfWeek = "Tuesday",   StartTime = new TimeOnly(10,0), EndTime = new TimeOnly(12,0), StudySession = "Day", RoomId = rooms[1].Id },
                new ScheduleEntry { CourseId = courses[2].Id, DayOfWeek = "Wednesday", StartTime = new TimeOnly(13,0), EndTime = new TimeOnly(15,0), StudySession = "Day", RoomId = rooms[2].Id },
                new ScheduleEntry { CourseId = courses[3].Id, DayOfWeek = "Thursday",  StartTime = new TimeOnly(8,0),  EndTime = new TimeOnly(10,0), StudySession = "Day", RoomId = rooms[3].Id }
            );
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string password,
        string role)
    {
        if (await userManager.FindByNameAsync(user.UserName!) is null)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
