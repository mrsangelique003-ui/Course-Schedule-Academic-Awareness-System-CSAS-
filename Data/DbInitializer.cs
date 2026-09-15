using CourseScheduleSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();

        if (await db.Students.AnyAsync())
            return;

        db.Students.AddRange(
            new Student
            {
                RegNo = "STU001",
                FullName = "Alice Uwimana",
                Email = "alice@unilak.ac.rw",
                Department = "CIS",
                Program = "Computer Information Systems",
                Level = "Year 4",
                StudySession = StudySession.Day
            }
        );

        db.ClassRepresentatives.AddRange(
            new ClassRepresentative
            {
                RegNo = "CP001",
                FullName = "Bob Nkurunziza",
                Email = "bob@unilak.ac.rw",
                Department = "CIS",
                StudySession = StudySession.Day
            }
        );

        db.Administrators.AddRange(
            new Administrator
            {
                StaffId = "ADM001",
                FullName = "Prof. Denis Habimana",
                Email = "denis@unilak.ac.rw",
                Department = "CIS",
                Role = "Dean"
            },
            new Administrator
            {
                StaffId = "ADM002",
                FullName = "Prof. Eric Mugisha",
                Email = "eric@unilak.ac.rw",
                Department = "CIS",
                Role = "HOD"
            },
            new Administrator
            {
                StaffId = "ADM003",
                FullName = "Dr. Claire Mukamana",
                Email = "claire@unilak.ac.rw",
                Department = "CIS",
                Role = "DirectorOfQuality"
            }
        );

        db.Lecturers.AddRange(
            new Lecturer
            {
                StaffId = "LEC001",
                FullName = "Dr. Jean Mugisha",
                Email = "j.mugisha@unilak.ac.rw",
                Department = "CIS"
            },
            new Lecturer
            {
                StaffId = "LEC002",
                FullName = "Mrs. Anne Byukusenge",
                Email = "a.byukusenge@unilak.ac.rw",
                Department = "CIS"
            },
            new Lecturer
            {
                StaffId = "LEC003",
                FullName = "Mr. Emmanuel Niyonsenga",
                Email = "e.niyonsenga@unilak.ac.rw",
                Department = "CIS"
            },
            new Lecturer
            {
                StaffId = "LEC004",
                FullName = "Dr. Patrick Habimana",
                Email = "p.habimana@unilak.ac.rw",
                Department = "CIS"
            }
        );

        db.Rooms.AddRange(
            new Room
            {
                RoomNumber = "A204",
                Building = "Block A",
                Capacity = 60,
                RoomType = "Classroom"
            },
            new Room
            {
                RoomNumber = "B101",
                Building = "Block B",
                Capacity = 80,
                RoomType = "Classroom"
            },
            new Room
            {
                RoomNumber = "A108",
                Building = "Block A",
                Capacity = 40,
                RoomType = "Classroom"
            },
            new Room
            {
                RoomNumber = "C302",
                Building = "Block C",
                Capacity = 50,
                RoomType = "Classroom"
            },
            new Room
            {
                RoomNumber = "LAB01",
                Building = "Block D",
                Capacity = 30,
                RoomType = "Computer Lab"
            }
        );

        await db.SaveChangesAsync();

        var lecturers = await db.Lecturers
            .OrderBy(l => l.Id)
            .ToListAsync();

        db.Courses.AddRange(
            new Course
            {
                Code = "CSE301",
                Name = "Software Engineering",
                Description = "Principles and practices of software engineering.",
                Credits = 3,
                Status = CourseStatus.Available,
                StudySession = StudySession.Day,
                LecturerId = lecturers[0].Id,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 12, 15),
                RegistrationOpenDate = new DateTime(2026, 8, 25),
                RegistrationCloseDate = new DateTime(2026, 9, 4)
            },
            new Course
            {
                Code = "CSE305",
                Name = "Database Systems",
                Description = "Database design, implementation and management.",
                Credits = 3,
                Status = CourseStatus.Available,
                StudySession = StudySession.Day,
                LecturerId = lecturers[1].Id,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 12, 15),
                RegistrationOpenDate = new DateTime(2026, 8, 25),
                RegistrationCloseDate = new DateTime(2026, 9, 4)
            },
            new Course
            {
                Code = "CSE310",
                Name = "Human-Computer Interaction",
                Description = "Design and evaluation of user-centered interfaces.",
                Credits = 3,
                Status = CourseStatus.Available,
                StudySession = StudySession.Day,
                LecturerId = lecturers[2].Id,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 12, 15),
                RegistrationOpenDate = new DateTime(2026, 8, 25),
                RegistrationCloseDate = new DateTime(2026, 9, 4)
            },
            new Course
            {
                Code = "CSE402",
                Name = "Software Architecture",
                Description = "Software architecture principles and system design.",
                Credits = 3,
                Status = CourseStatus.Available,
                StudySession = StudySession.Day,
                LecturerId = lecturers[3].Id,
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 12, 15),
                RegistrationOpenDate = new DateTime(2026, 8, 25),
                RegistrationCloseDate = new DateTime(2026, 9, 4)
            }
        );

        await db.SaveChangesAsync();

        var courses = await db.Courses
            .OrderBy(c => c.Id)
            .ToListAsync();

        var rooms = await db.Rooms
            .OrderBy(r => r.Id)
            .ToListAsync();

        db.ScheduleEntries.AddRange(
            new ScheduleEntry
            {
                CourseId = courses[0].Id,
                LecturerId = lecturers[0].Id,
                RoomId = rooms[0].Id,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(10, 0),
                StudySession = StudySession.Day,
                Status = ScheduleStatus.Active
            },
            new ScheduleEntry
            {
                CourseId = courses[1].Id,
                LecturerId = lecturers[1].Id,
                RoomId = rooms[1].Id,
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0),
                StudySession = StudySession.Day,
                Status = ScheduleStatus.Active
            },
            new ScheduleEntry
            {
                CourseId = courses[2].Id,
                LecturerId = lecturers[2].Id,
                RoomId = rooms[2].Id,
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = new TimeOnly(13, 0),
                EndTime = new TimeOnly(15, 0),
                StudySession = StudySession.Day,
                Status = ScheduleStatus.Active
            },
            new ScheduleEntry
            {
                CourseId = courses[3].Id,
                LecturerId = lecturers[3].Id,
                RoomId = rooms[3].Id,
                DayOfWeek = DayOfWeek.Thursday,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(10, 0),
                StudySession = StudySession.Day,
                Status = ScheduleStatus.Active
            }
        );

        await db.SaveChangesAsync();
    }
}