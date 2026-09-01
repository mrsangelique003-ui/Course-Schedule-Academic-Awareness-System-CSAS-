using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        // Apply pending migrations
        await db.Database.MigrateAsync();

        // ── FAST EXIT: DB already seeded — skip everything ─────────────
        // Check one fast query — if any AspNetUsers rows exist, we're done
        if (await db.Users.AnyAsync()) return;

        // ── Only runs on first-ever startup ────────────────────────────
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Roles
        foreach (var r in new[] { "Student", "CP", "DirectorOfQuality", "Dean", "HOD" })
            await roleManager.CreateAsync(new ApplicationRole(r));

        // 2. Demo users
        await CreateUser(userManager, "STU001",  "Alice Uwimana",        "alice@unilak.ac.rw",  "Day",  "Student");
        await CreateUser(userManager, "CP001",   "Bob Nkurunziza",       "bob@unilak.ac.rw",    "Day",  "CP");
        await CreateUser(userManager, "DIR001",  "Dr. Claire Mukamana",  "claire@unilak.ac.rw", "Day",  "DirectorOfQuality");
        await CreateUser(userManager, "DEAN001", "Prof. Denis Habimana", "denis@unilak.ac.rw",  "Day",  "Dean");
        await CreateUser(userManager, "HOD001",  "Prof. Eric Mugisha",   "eric@unilak.ac.rw",   "Day",  "HOD");

        // 3. Lecturers
        db.Lecturers.AddRange(
            new Lecturer { StaffId = "LEC001", FullName = "Dr. Jean Mugisha",         Email = "j.mugisha@unilak.ac.rw",    Department = "CIS" },
            new Lecturer { StaffId = "LEC002", FullName = "Mrs. Anne Byukusenge",     Email = "a.byukusenge@unilak.ac.rw", Department = "CIS" },
            new Lecturer { StaffId = "LEC003", FullName = "Mr. Emmanuel Niyonsenga", Email = "e.niyonsenga@unilak.ac.rw", Department = "CIS" },
            new Lecturer { StaffId = "LEC004", FullName = "Dr. Patrick Habimana",    Email = "p.habimana@unilak.ac.rw",   Department = "CIS" }
        );
        await db.SaveChangesAsync();

        // 4. Rooms
        db.Rooms.AddRange(
            new Room { RoomNumber = "A204",  Building = "Block A", Capacity = 60 },
            new Room { RoomNumber = "B101",  Building = "Block B", Capacity = 80 },
            new Room { RoomNumber = "A108",  Building = "Block A", Capacity = 40 },
            new Room { RoomNumber = "C302",  Building = "Block C", Capacity = 50 },
            new Room { RoomNumber = "LAB01", Building = "Block D", Capacity = 30 }
        );
        await db.SaveChangesAsync();

        // 5. Courses
        var lecs = await db.Lecturers.OrderBy(l => l.Id).Select(l => l.Id).ToListAsync();
        db.Courses.AddRange(
            new Course { Code = "CSE301", Name = "Software Engineering",       LecturerId = lecs[0], Status = "Available", WhatsAppGroupLink = "https://chat.whatsapp.com/example1", StartDate = new DateTime(2026,9,1), EndDate = new DateTime(2026,12,15), RegistrationOpenDate = new DateTime(2026,8,25), RegistrationCloseDate = new DateTime(2026,9,4) },
            new Course { Code = "CSE305", Name = "Database Systems",           LecturerId = lecs[1], Status = "Available", WhatsAppGroupLink = "https://chat.whatsapp.com/example2", StartDate = new DateTime(2026,9,1), EndDate = new DateTime(2026,12,15), RegistrationOpenDate = new DateTime(2026,8,25), RegistrationCloseDate = new DateTime(2026,9,4) },
            new Course { Code = "CSE310", Name = "Human-Computer Interaction", LecturerId = lecs[2], Status = "Available", WhatsAppGroupLink = "https://chat.whatsapp.com/example3", StartDate = new DateTime(2026,9,1), EndDate = new DateTime(2026,12,15), RegistrationOpenDate = new DateTime(2026,8,25), RegistrationCloseDate = new DateTime(2026,9,4) },
            new Course { Code = "CSE402", Name = "Software Architecture",      LecturerId = lecs[3], Status = "Available", WhatsAppGroupLink = "https://chat.whatsapp.com/example4", StartDate = new DateTime(2026,9,1), EndDate = new DateTime(2026,12,15), RegistrationOpenDate = new DateTime(2026,8,25), RegistrationCloseDate = new DateTime(2026,9,4) }
        );
        await db.SaveChangesAsync();

        // 6. Schedule entries
        var cIds = await db.Courses.OrderBy(c => c.Id).Select(c => c.Id).ToListAsync();
        var rIds = await db.Rooms.OrderBy(r => r.Id).Select(r => r.Id).ToListAsync();
        db.ScheduleEntries.AddRange(
            new ScheduleEntry { CourseId = cIds[0], RoomId = rIds[0], DayOfWeek = "Monday",    StudySession = "Day", StartTime = new TimeOnly(8,0),  EndTime = new TimeOnly(10,0) },
            new ScheduleEntry { CourseId = cIds[1], RoomId = rIds[1], DayOfWeek = "Tuesday",   StudySession = "Day", StartTime = new TimeOnly(10,0), EndTime = new TimeOnly(12,0) },
            new ScheduleEntry { CourseId = cIds[2], RoomId = rIds[2], DayOfWeek = "Wednesday", StudySession = "Day", StartTime = new TimeOnly(13,0), EndTime = new TimeOnly(15,0) },
            new ScheduleEntry { CourseId = cIds[3], RoomId = rIds[3], DayOfWeek = "Thursday",  StudySession = "Day", StartTime = new TimeOnly(8,0),  EndTime = new TimeOnly(10,0) }
        );
        await db.SaveChangesAsync();
    }

    private static async Task CreateUser(
        UserManager<ApplicationUser> um,
        string regNo, string fullName, string email,
        string session, string role)
    {
        var user = new ApplicationUser
        {
            UserName = regNo, RegNo = regNo, FullName = fullName,
            Email = email, Department = "CIS",
            StudySession = session, IsActive = true
        };
        var result = await um.CreateAsync(user, "Pass@123");
        if (result.Succeeded) await um.AddToRoleAsync(user, role);
    }
}
