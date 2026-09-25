using CourseScheduleSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleSystem.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();


    await db.Database.MigrateAsync();

        const string defaultPassword = "M00del!!";

        var studentHasher = new PasswordHasher<Student>();
        var classRepresentativeHasher =
            new PasswordHasher<ClassRepresentative>();
        var administratorHasher = new PasswordHasher<Administrator>();
        var lecturerHasher = new PasswordHasher<Lecturer>();

        var now = DateTime.UtcNow;

        var currentDate = now.Date;

        var daysFromMonday =
            ((int)currentDate.DayOfWeek + 6) % 7;

        var teachingStart =
            currentDate.AddDays(-daysFromMonday);

        var teachingEnd =
            teachingStart.AddDays(13);

        // =========================================================
        // STUDENTS
        // =========================================================

        var studentData = new[]
        {
        ("STU001", "Alice Uwimana", "alice@unilak.ac.rw"),
        ("STU002", "Brian Niyonzima", "brian@unilak.ac.rw"),
        ("STU003", "Chantal Mukamana", "chantal@unilak.ac.rw"),
        ("STU004", "David Habimana", "david@unilak.ac.rw"),
        ("STU005", "Esther Ingabire", "esther@unilak.ac.rw"),
        ("STU006", "Fabrice Nshimiyimana", "fabrice@unilak.ac.rw"),
        ("STU007", "Grace Uwamahoro", "grace@unilak.ac.rw"),
        ("STU008", "Herve Tuyisenge", "herve@unilak.ac.rw"),
        ("STU009", "Immaculate Mukeshimana", "immaculate@unilak.ac.rw"),
        ("STU010", "Jean Claude Nkurunziza", "jeanclaude@unilak.ac.rw"),
        ("STU011", "Kevin Nsengiyumva", "kevin@unilak.ac.rw"),
        ("STU012", "Liliane Uwase", "liliane@unilak.ac.rw"),
        ("STU013", "Martin Ndayisenga", "martin@unilak.ac.rw"),
        ("STU014", "Nadine Mukamana", "nadine@unilak.ac.rw"),
        ("STU015", "Olivier Bizimana", "olivier@unilak.ac.rw"),
        ("STU016", "Peace Uwamariya", "peace@unilak.ac.rw"),
        ("STU017", "Richard Mugabo", "richard@unilak.ac.rw"),
        ("STU018", "Sarah Ingabire", "sarah@unilak.ac.rw"),
        ("STU019", "Theogene Niyomugabo", "theogene@unilak.ac.rw"),
        ("STU020", "Yvette Mukamana", "yvette@unilak.ac.rw")
    };

        foreach (var item in studentData)
        {
            var student =
                await db.Students
                    .FirstOrDefaultAsync(s => s.RegNo == item.Item1);

            if (student == null)
            {
                student = new Student
                {
                    RegNo = item.Item1,
                    FullName = item.Item2,
                    Email = item.Item3,
                    Department = "CIS",
                    Program = "Computer Information Systems",
                    Level = "Year 4",
                    StudySession = StudySession.Day,
                    IsActive = true
                };

                student.PasswordHash =
                    studentHasher.HashPassword(
                        student,
                        defaultPassword);

                db.Students.Add(student);
            }
            else
            {
                student.FullName = item.Item2;
                student.Email = item.Item3;
                student.Department = "CIS";
                student.Program = "Computer Information Systems";
                student.Level = "Year 4";
                student.IsActive = true;

                if (string.IsNullOrWhiteSpace(student.PasswordHash))
                {
                    student.PasswordHash =
                        studentHasher.HashPassword(
                            student,
                            defaultPassword);
                }
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // CLASS REPRESENTATIVES
        // =========================================================

        var classRepresentativeData = new[]
        {
        ("CP001", "Bob Nkurunziza", "bob@unilak.ac.rw", "0788000001"),
        ("CP002", "Clarisse Uwimana", "clarisse@unilak.ac.rw", "0788000002"),
        ("CP003", "Derrick Niyonsenga", "derrick@unilak.ac.rw", "0788000003"),
        ("CP004", "Emmanuel Habimana", "emmanuel@unilak.ac.rw", "0788000004"),
        ("CP005", "Florence Mukamana", "florence@unilak.ac.rw", "0788000005"),
        ("CP006", "Gaspard Bizimana", "gaspard@unilak.ac.rw", "0788000006"),
        ("CP007", "Hope Uwamahoro", "hope@unilak.ac.rw", "0788000007"),
        ("CP008", "Ivan Nshimiyimana", "ivan@unilak.ac.rw", "0788000008"),
        ("CP009", "Josiane Ingabire", "josiane@unilak.ac.rw", "0788000009"),
        ("CP010", "Kenneth Mugisha", "kenneth@unilak.ac.rw", "0788000010"),
        ("CP011", "Lydia Uwase", "lydia@unilak.ac.rw", "0788000011"),
        ("CP012", "Michel Ndayisenga", "michel@unilak.ac.rw", "0788000012"),
        ("CP013", "Nathalie Mukeshimana", "nathalie@unilak.ac.rw", "0788000013"),
        ("CP014", "Oscar Tuyisenge", "oscar@unilak.ac.rw", "0788000014"),
        ("CP015", "Patricia Uwamariya", "patricia@unilak.ac.rw", "0788000015"),
        ("CP016", "Robert Niyomugabo", "robert@unilak.ac.rw", "0788000016"),
        ("CP017", "Sandrine Mukamana", "sandrine@unilak.ac.rw", "0788000017"),
        ("CP018", "Theoneste Habimana", "theoneste@unilak.ac.rw", "0788000018"),
        ("CP019", "Valerie Ingabire", "valerie@unilak.ac.rw", "0788000019"),
        ("CP020", "William Nkurunziza", "william@unilak.ac.rw", "0788000020")
    };

        foreach (var item in classRepresentativeData)
        {
            var representative =
                await db.ClassRepresentatives
                    .FirstOrDefaultAsync(
                        cp => cp.RegNo == item.Item1);

            if (representative == null)
            {
                representative = new ClassRepresentative
                {
                    RegNo = item.Item1,
                    FullName = item.Item2,
                    Email = item.Item3,
                    PhoneNumber = item.Item4,
                    Nationality = "Rwandan",
                    Department = "Software Engineering",
                    Faculty = "Information Technology",
                    Year = 3,
                    StudySession = StudySession.Day,
                    IsActive = true,
                    Intake = "September",
                    Level = "Year 3"
                };

                representative.PasswordHash =
                    classRepresentativeHasher.HashPassword(
                        representative,
                        defaultPassword);

                db.ClassRepresentatives.Add(representative);
            }
            else
            {
                representative.FullName = item.Item2;
                representative.Email = item.Item3;
                representative.PhoneNumber = item.Item4;
                representative.Nationality = "Rwandan";
                representative.Department = "Software Engineering";
                representative.Faculty = "Information Technology";
                representative.Year = 3;
                representative.StudySession = StudySession.Day;
                representative.IsActive = true;
                representative.Intake = "September";
                representative.Level = "Year 3";

                if (string.IsNullOrWhiteSpace(
                    representative.PasswordHash))
                {
                    representative.PasswordHash =
                        classRepresentativeHasher.HashPassword(
                            representative,
                            defaultPassword);
                }
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // ADMINISTRATORS
        // =========================================================

        var administratorData = new[]
        {
        ("ADM001", "Prof. Denis Habimana", "denis@unilak.ac.rw", "Dean"),
        ("ADM002", "Prof. Eric Mugisha", "eric@unilak.ac.rw", "HOD"),
        ("ADM003", "Dr. Claire Mukamana", "claire@unilak.ac.rw", "DirectorOfQuality"),
        ("ADM004", "Dr. Alice Mukamana", "alice.admin@unilak.ac.rw", "Dean"),
        ("ADM005", "Dr. Bernard Niyonzima", "bernard@unilak.ac.rw", "HOD"),
        ("ADM006", "Dr. Chantal Uwase", "chantal.admin@unilak.ac.rw", "HOD"),
        ("ADM007", "Prof. Daniel Nshimiyimana", "daniel@unilak.ac.rw", "Dean"),
        ("ADM008", "Dr. Emmanuel Mugabo", "emmanuel.admin@unilak.ac.rw", "HOD"),
        ("ADM009", "Dr. Florence Ingabire", "florence.admin@unilak.ac.rw", "DirectorOfQuality"),
        ("ADM010", "Prof. George Habimana", "george@unilak.ac.rw", "Dean"),
        ("ADM011", "Dr. Helene Mukamana", "helene@unilak.ac.rw", "HOD"),
        ("ADM012", "Dr. Isaac Nkurunziza", "isaac@unilak.ac.rw", "HOD"),
        ("ADM013", "Prof. Jacqueline Uwimana", "jacqueline@unilak.ac.rw", "Dean"),
        ("ADM014", "Dr. Kevin Bizimana", "kevin.admin@unilak.ac.rw", "HOD"),
        ("ADM015", "Dr. Louise Niyomugabo", "louise@unilak.ac.rw", "DirectorOfQuality"),
        ("ADM016", "Prof. Michael Tuyisenge", "michael@unilak.ac.rw", "Dean"),
        ("ADM017", "Dr. Nicole Uwamahoro", "nicole@unilak.ac.rw", "HOD"),
        ("ADM018", "Dr. Olivier Ndayisenga", "olivier.admin@unilak.ac.rw", "HOD"),
        ("ADM019", "Prof. Patrick Mugisha", "patrick@unilak.ac.rw", "Dean"),
        ("ADM020", "Dr. Sarah Ingabire", "sarah.admin@unilak.ac.rw", "DirectorOfQuality")
    };

        foreach (var item in administratorData)
        {
            var administrator =
                await db.Administrators
                    .FirstOrDefaultAsync(
                        a => a.StaffId == item.Item1);

            if (administrator == null)
            {
                administrator = new Administrator
                {
                    StaffId = item.Item1,
                    FullName = item.Item2,
                    Email = item.Item3,
                    Department = "CIS",
                    Role = item.Item4,
                    IsActive = true
                };

                administrator.PasswordHash =
                    administratorHasher.HashPassword(
                        administrator,
                        defaultPassword);

                db.Administrators.Add(administrator);
            }
            else
            {
                administrator.FullName = item.Item2;
                administrator.Email = item.Item3;
                administrator.Department = "CIS";
                administrator.Role = item.Item4;
                administrator.IsActive = true;

                if (string.IsNullOrWhiteSpace(
                    administrator.PasswordHash))
                {
                    administrator.PasswordHash =
                        administratorHasher.HashPassword(
                            administrator,
                            defaultPassword);
                }
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // LECTURERS
        // =========================================================

        var lecturerData = new[]
        {
        ("LEC001", "Dr. Jean Mugisha", "j.mugisha@unilak.ac.rw"),
        ("LEC002", "Mrs. Anne Byukusenge", "a.byukusenge@unilak.ac.rw"),
        ("LEC003", "Mr. Emmanuel Niyonsenga", "e.niyonsenga@unilak.ac.rw"),
        ("LEC004", "Dr. Patrick Habimana", "p.habimana@unilak.ac.rw"),
        ("LEC005", "Dr. Alice Uwase", "alice.lecturer@unilak.ac.rw"),
        ("LEC006", "Mr. Bernard Nkurunziza", "bernard.lecturer@unilak.ac.rw"),
        ("LEC007", "Dr. Chantal Ingabire", "chantal.lecturer@unilak.ac.rw"),
        ("LEC008", "Mr. David Mugabo", "david.lecturer@unilak.ac.rw"),
        ("LEC009", "Dr. Esther Mukamana", "esther.lecturer@unilak.ac.rw"),
        ("LEC010", "Mr. Fabrice Nshimiyimana", "fabrice.lecturer@unilak.ac.rw"),
        ("LEC011", "Dr. Grace Uwamahoro", "grace.lecturer@unilak.ac.rw"),
        ("LEC012", "Mr. Herve Tuyisenge", "herve.lecturer@unilak.ac.rw"),
        ("LEC013", "Dr. Immaculate Mukeshimana", "immaculate.lecturer@unilak.ac.rw"),
        ("LEC014", "Mr. Joseph Ndayisenga", "joseph.lecturer@unilak.ac.rw"),
        ("LEC015", "Dr. Liliane Uwase", "liliane.lecturer@unilak.ac.rw"),
        ("LEC016", "Mr. Martin Bizimana", "martin.lecturer@unilak.ac.rw"),
        ("LEC017", "Dr. Nadine Mukamana", "nadine.lecturer@unilak.ac.rw"),
        ("LEC018", "Mr. Olivier Niyomugabo", "olivier.lecturer@unilak.ac.rw"),
        ("LEC019", "Dr. Peace Uwamariya", "peace.lecturer@unilak.ac.rw"),
        ("LEC020", "Mr. Richard Mugisha", "richard.lecturer@unilak.ac.rw")
    };

        foreach (var item in lecturerData)
        {
            var lecturer =
                await db.Lecturers
                    .FirstOrDefaultAsync(
                        l => l.StaffId == item.Item1);

            if (lecturer == null)
            {
                lecturer = new Lecturer
                {
                    StaffId = item.Item1,
                    FullName = item.Item2,
                    Email = item.Item3,
                    PhoneNumber = $"078811{item.Item1.Substring(3)}",
                    Department = "CIS",
                    IsActive = true
                };

                lecturer.PasswordHash =
                    lecturerHasher.HashPassword(
                        lecturer,
                        defaultPassword);

                db.Lecturers.Add(lecturer);
            }
            else
            {
                lecturer.FullName = item.Item2;
                lecturer.Email = item.Item3;
                lecturer.PhoneNumber =
                    $"078811{item.Item1.Substring(3)}";
                lecturer.Department = "CIS";
                lecturer.IsActive = true;

                if (string.IsNullOrWhiteSpace(
                    lecturer.PasswordHash))
                {
                    lecturer.PasswordHash =
                        lecturerHasher.HashPassword(
                            lecturer,
                            defaultPassword);
                }
            }
        }

        await db.SaveChangesAsync();

        var lecturers =
            await db.Lecturers
                .OrderBy(l => l.Id)
                .Take(20)
                .ToListAsync();

        // =========================================================
        // ROOMS
        // =========================================================

        var roomData = new[]
        {
        ("A204", "Block A", 60, "Classroom"),
        ("B101", "Block B", 80, "Classroom"),
        ("A108", "Block A", 40, "Classroom"),
        ("C302", "Block C", 50, "Classroom"),
        ("LAB01", "Block D", 30, "Computer Lab"),
        ("A205", "Block A", 55, "Classroom"),
        ("A206", "Block A", 45, "Classroom"),
        ("B102", "Block B", 70, "Classroom"),
        ("B103", "Block B", 60, "Classroom"),
        ("B201", "Block B", 90, "Lecture Hall"),
        ("B202", "Block B", 75, "Classroom"),
        ("C301", "Block C", 55, "Classroom"),
        ("C303", "Block C", 45, "Classroom"),
        ("C304", "Block C", 60, "Classroom"),
        ("LAB02", "Block D", 35, "Computer Lab"),
        ("LAB03", "Block D", 40, "Computer Lab"),
        ("D101", "Block D", 50, "Classroom"),
        ("D102", "Block D", 65, "Classroom"),
        ("E201", "Block E", 100, "Lecture Hall"),
        ("E202", "Block E", 80, "Classroom")
    };

        foreach (var item in roomData)
        {
            var room =
                await db.Rooms
                    .FirstOrDefaultAsync(
                        r => r.RoomNumber == item.Item1);

            if (room == null)
            {
                db.Rooms.Add(
                    new Room
                    {
                        RoomNumber = item.Item1,
                        Building = item.Item2,
                        Capacity = item.Item3,
                        RoomType = item.Item4,
                        IsAvailable = true
                    });
            }
            else
            {
                room.Building = item.Item2;
                room.Capacity = item.Item3;
                room.RoomType = item.Item4;
                room.IsAvailable = true;
            }
        }

        await db.SaveChangesAsync();

        var rooms =
            await db.Rooms
                .OrderBy(r => r.Id)
                .Take(20)
                .ToListAsync();

        // =========================================================
        // COURSES
        // =========================================================

        var courseData = new[]
        {
        ("CSE301", "Software Engineering", 3, "Principles and practices of software engineering."),
        ("CSE305", "Database Systems", 3, "Database design, implementation and management."),
        ("CSE310", "Human-Computer Interaction", 3, "Design and evaluation of user-centered interfaces."),
        ("CSE402", "Software Architecture", 3, "Software architecture principles and system design."),
        ("CSE303", "Data Structures and Algorithms", 3, "Algorithms, complexity and fundamental data structures."),
        ("CSE304", "Computer Networks", 3, "Network architecture, protocols and communication."),
        ("CSE306", "Operating Systems", 3, "Operating system concepts, processes and memory management."),
        ("CSE307", "Web Application Development", 3, "Modern web application development techniques."),
        ("CSE308", "Mobile Application Development", 3, "Development of applications for mobile platforms."),
        ("CSE309", "Information Security", 3, "Principles of cybersecurity and information protection."),
        ("CSE311", "Systems Analysis and Design", 3, "Analysis, modelling and design of information systems."),
        ("CSE312", "Artificial Intelligence", 3, "Fundamental concepts and applications of artificial intelligence."),
        ("CSE313", "Machine Learning", 3, "Introduction to machine learning algorithms and applications."),
        ("CSE314", "Cloud Computing", 3, "Cloud infrastructure, services and deployment models."),
        ("CSE315", "Software Testing", 3, "Software verification, validation and testing techniques."),
        ("CSE401", "Project Management", 3, "Planning, execution and control of software projects."),
        ("CSE403", "Distributed Systems", 3, "Distributed computing concepts and architectures."),
        ("CSE404", "Enterprise Systems", 3, "Design and management of enterprise information systems."),
        ("CSE405", "Research Methods", 3, "Research design, methods and academic investigation."),
        ("CSE406", "Final Year Project", 6, "Planning, development and presentation of a software project.")
    };

        var registrationOpen =
            teachingStart.AddDays(-7);

        var registrationClose =
            teachingStart.AddDays(-1);

        for (var i = 0; i < courseData.Length; i++)
        {
            var item = courseData[i];

            var session =
                i < 7
                    ? StudySession.Day
                    : i < 14
                        ? StudySession.Evening
                        : StudySession.Weekend;

            var course =
                await db.Courses
                    .FirstOrDefaultAsync(
                        c => c.Code == item.Item1);

            if (course == null)
            {
                course = new Course
                {
                    Code = item.Item1,
                    Name = item.Item2,
                    Description = item.Item4,
                    Credits = item.Item3,
                    Status = CourseStatus.Available,
                    StudySession = session,
                    LecturerId = lecturers[i].Id,
                    StartDate = teachingStart,
                    EndDate = teachingEnd,
                    RegistrationOpenDate = registrationOpen,
                    RegistrationCloseDate = registrationClose,
                    IsActive = true
                };

                db.Courses.Add(course);
            }
            else
            {
                course.Name = item.Item2;
                course.Description = item.Item4;
                course.Credits = item.Item3;
                course.Status = CourseStatus.Available;
                course.StudySession = session;
                course.LecturerId = lecturers[i].Id;
                course.StartDate = teachingStart;
                course.EndDate = teachingEnd;
                course.RegistrationOpenDate = registrationOpen;
                course.RegistrationCloseDate = registrationClose;
                course.IsActive = true;
            }
        }

        await db.SaveChangesAsync();

        var courses =
            await db.Courses
                .OrderBy(c => c.Id)
                .Take(20)
                .ToListAsync();

        var representatives =
            await db.ClassRepresentatives
                .OrderBy(cp => cp.Id)
                .Take(20)
                .ToListAsync();

        // =========================================================
        // SCHEDULES
        //
        // 7 DAY
        // 7 EVENING
        // 6 WEEKEND
        //
        // Every schedule belongs to the same two-week course period.
        // =========================================================

        var scheduleDefinitions = new[]
        {
        (0, DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (1, DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (2, DayOfWeek.Tuesday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (3, DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (4, DayOfWeek.Wednesday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (5, DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (6, DayOfWeek.Friday, new TimeOnly(8, 0), new TimeOnly(10, 0)),

        (7, DayOfWeek.Monday, new TimeOnly(17, 0), new TimeOnly(19, 0)),
        (8, DayOfWeek.Monday, new TimeOnly(19, 0), new TimeOnly(21, 0)),
        (9, DayOfWeek.Tuesday, new TimeOnly(17, 0), new TimeOnly(19, 0)),
        (10, DayOfWeek.Tuesday, new TimeOnly(19, 0), new TimeOnly(21, 0)),
        (11, DayOfWeek.Wednesday, new TimeOnly(17, 0), new TimeOnly(19, 0)),
        (12, DayOfWeek.Thursday, new TimeOnly(17, 0), new TimeOnly(19, 0)),
        (13, DayOfWeek.Thursday, new TimeOnly(19, 0), new TimeOnly(21, 0)),

        (14, DayOfWeek.Sunday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (15, DayOfWeek.Sunday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (16, DayOfWeek.Sunday, new TimeOnly(12, 0), new TimeOnly(14, 0)),
        (17, DayOfWeek.Sunday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (18, DayOfWeek.Sunday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (19, DayOfWeek.Sunday, new TimeOnly(12, 0), new TimeOnly(14, 0))
    };

        var existingSchedules =
            await db.ScheduleEntries
                .ToListAsync();

        foreach (var definition in scheduleDefinitions)
        {
            var course =
                courses[definition.Item1];

            var lecturer =
                lecturers[definition.Item1];

            var representative =
                representatives[definition.Item1];

            var room =
                rooms[definition.Item1];

            var expectedSession =
                definition.Item1 < 7
                    ? StudySession.Day
                    : definition.Item1 < 14
                        ? StudySession.Evening
                        : StudySession.Weekend;

            var schedule =
                existingSchedules
                    .FirstOrDefault(s =>
                        s.CourseId == course.Id);

            if (schedule == null)
            {
                schedule = new ScheduleEntry
                {
                    CourseId = course.Id,
                    LecturerId = lecturer.Id,
                    ClassRepresentativeId = representative.Id,
                    RoomId = room.Id,
                    DayOfWeek = definition.Item2,
                    StartTime = definition.Item3,
                    EndTime = definition.Item4,
                    StartDate = teachingStart,
                    EndDate = teachingEnd,
                    StudySession = expectedSession,
                    Status = ScheduleStatus.Active,
                    Notes = $"Regular {course.Code} class",
                    IsActive = true,
                    CreatedAt = now
                };

                db.ScheduleEntries.Add(schedule);
            }
            else
            {
                schedule.CourseId = course.Id;
                schedule.LecturerId = lecturer.Id;
                schedule.ClassRepresentativeId = representative.Id;
                schedule.RoomId = room.Id;
                schedule.DayOfWeek = definition.Item2;
                schedule.StartTime = definition.Item3;
                schedule.EndTime = definition.Item4;
                schedule.StartDate = teachingStart;
                schedule.EndDate = teachingEnd;
                schedule.StudySession = expectedSession;
                schedule.Status = ScheduleStatus.Active;
                schedule.Notes = $"Regular {course.Code} class";
                schedule.IsActive = true;
                schedule.UpdatedAt = now;
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // ENROLLMENTS
        // =========================================================

        var students =
            await db.Students
                .OrderBy(s => s.Id)
                .Take(20)
                .ToListAsync();

        for (var i = 0;
             i < 20 &&
             i < students.Count &&
             i < courses.Count;
             i++)
        {
            var student = students[i];
            var course = courses[i];

            var enrollment =
                await db.Enrollments
                    .FirstOrDefaultAsync(e =>
                        e.StudentId == student.Id &&
                        e.CourseId == course.Id);

            var status =
                i < 15
                    ? EnrollmentStatus.Enrolled
                    : EnrollmentStatus.Closed;

            if (enrollment == null)
            {
                db.Enrollments.Add(
                    new Enrollment
                    {
                        StudentId = student.Id,
                        CourseId = course.Id,
                        Status = status,
                        RequestedAt =
                            now.AddDays(-(i + 1)),
                        ApprovedAt =
                            status == EnrollmentStatus.Enrolled
                                ? now.AddDays(-(i + 1))
                                : null
                    });
            }
            else
            {
                enrollment.Status = status;

                enrollment.RequestedAt =
                    now.AddDays(-(i + 1));

                enrollment.ApprovedAt =
                    status == EnrollmentStatus.Enrolled
                        ? now.AddDays(-(i + 1))
                        : null;
            }
        }

        var firstStudent =
            students.FirstOrDefault(
                s => s.RegNo == "STU001");

        if (firstStudent != null)
        {
            var firstFiveCourses =
                courses.Take(5).ToList();

            foreach (var course in firstFiveCourses)
            {
                var enrollment =
                    db.Enrollments.Local
                        .FirstOrDefault(e =>
                            e.StudentId == firstStudent.Id &&
                            e.CourseId == course.Id);

                if (enrollment == null)
                {
                    enrollment =
                        await db.Enrollments
                            .FirstOrDefaultAsync(e =>
                                e.StudentId == firstStudent.Id &&
                                e.CourseId == course.Id);
                }

                if (enrollment == null)
                {
                    db.Enrollments.Add(
                        new Enrollment
                        {
                            StudentId = firstStudent.Id,
                            CourseId = course.Id,
                            Status = EnrollmentStatus.Enrolled,
                            RequestedAt = now.AddDays(-1),
                            ApprovedAt = now
                        });
                }
                else
                {
                    enrollment.Status =
                        EnrollmentStatus.Enrolled;

                    enrollment.RequestedAt =
                        now.AddDays(-1);

                    enrollment.ApprovedAt =
                        now;
                }
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // EXAMS
        // =========================================================

        var examDefinitions = new[]
        {
        (
            CourseIndex: 0,
            ExamType: ExamType.CAT,
            DaysFromToday: 3,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(11, 0),
            RoomIndex: 0
        ),
        (
            CourseIndex: 1,
            ExamType: ExamType.CAT,
            DaysFromToday: 5,
            StartTime: new TimeOnly(13, 0),
            EndTime: new TimeOnly(15, 0),
            RoomIndex: 1
        ),
        (
            CourseIndex: 2,
            ExamType: ExamType.CAT,
            DaysFromToday: 8,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(11, 0),
            RoomIndex: 2
        ),
        (
            CourseIndex: 3,
            ExamType: ExamType.CAT,
            DaysFromToday: 10,
            StartTime: new TimeOnly(13, 0),
            EndTime: new TimeOnly(15, 0),
            RoomIndex: 3
        ),
        (
            CourseIndex: 4,
            ExamType: ExamType.CAT,
            DaysFromToday: 13,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(11, 0),
            RoomIndex: 4
        ),
        (
            CourseIndex: 0,
            ExamType: ExamType.Final,
            DaysFromToday: 21,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(12, 0),
            RoomIndex: 5
        ),
        (
            CourseIndex: 1,
            ExamType: ExamType.Final,
            DaysFromToday: 25,
            StartTime: new TimeOnly(13, 0),
            EndTime: new TimeOnly(16, 0),
            RoomIndex: 6
        ),
        (
            CourseIndex: 2,
            ExamType: ExamType.Final,
            DaysFromToday: 30,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(12, 0),
            RoomIndex: 7
        ),
        (
            CourseIndex: 3,
            ExamType: ExamType.Final,
            DaysFromToday: 35,
            StartTime: new TimeOnly(13, 0),
            EndTime: new TimeOnly(16, 0),
            RoomIndex: 8
        ),
        (
            CourseIndex: 4,
            ExamType: ExamType.Final,
            DaysFromToday: 40,
            StartTime: new TimeOnly(9, 0),
            EndTime: new TimeOnly(12, 0),
            RoomIndex: 9
        )
    };

        foreach (var definition in examDefinitions)
        {
            var course =
                courses[definition.CourseIndex];

            var room =
                rooms[definition.RoomIndex];

            var examDate =
                currentDate.AddDays(
                    definition.DaysFromToday);

            var exam =
                await db.Exams
                    .FirstOrDefaultAsync(e =>
                        e.CourseId == course.Id &&
                        e.ExamType == definition.ExamType);

            if (exam == null)
            {
                db.Exams.Add(
                    new Exam
                    {
                        CourseId = course.Id,
                        RoomId = room.Id,
                        ExamType = definition.ExamType,
                        ExamDate = examDate,
                        StartTime = definition.StartTime,
                        EndTime = definition.EndTime,
                        IsActive = true,
                        Notes =
                            $"{definition.ExamType} examination for {course.Code}",
                        CreatedAt = now
                    });
            }
            else
            {
                exam.RoomId = room.Id;
                exam.ExamDate = examDate;
                exam.StartTime = definition.StartTime;
                exam.EndTime = definition.EndTime;
                exam.IsActive = true;
                exam.Notes =
                    $"{definition.ExamType} examination for {course.Code}";
                exam.UpdatedAt = now;
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // SUPPORT TICKETS
        // =========================================================

        var supportTicketData = new[]
        {
        (
            "SUP-1001",
            0,
            "Cannot access my account",
            SupportCategory.AccountAccess,
            "I cannot sign in to my CSAS student account even though I am using the correct password.",
            SupportTicketStatus.Open
        ),
        (
            "SUP-1002",
            1,
            "Schedule shows the wrong classroom",
            SupportCategory.CourseSchedule,
            "The classroom shown for one of my scheduled classes appears to be different from the room communicated to our class.",
            SupportTicketStatus.InProgress
        ),
        (
            "SUP-1003",
            2,
            "Unable to view available courses",
            SupportCategory.CourseSchedule,
            "The available courses page does not display all courses that should be available to my class.",
            SupportTicketStatus.Open
        ),
        (
            "SUP-1004",
            3,
            "Enrollment information is missing",
            SupportCategory.Enrollment,
            "One of my enrolled courses is not appearing in my course list.",
            SupportTicketStatus.Resolved
        ),
        (
            "SUP-1005",
            4,
            "Exam timetable question",
            SupportCategory.Examination,
            "I need help understanding the date and room shown for my upcoming examination.",
            SupportTicketStatus.Closed
        ),
        (
            "SUP-1006",
            5,
            "Profile information needs correction",
            SupportCategory.AccountAccess,
            "My student profile contains information that needs to be reviewed and corrected.",
            SupportTicketStatus.Resolved
        ),
        (
            "SUP-1007",
            6,
            "Schedule page is not loading",
            SupportCategory.TechnicalIssue,
            "The schedule page takes too long to load and sometimes displays an empty page.",
            SupportTicketStatus.InProgress
        ),
        (
            "SUP-1008",
            7,
            "General academic support",
            SupportCategory.Other,
            "I need assistance understanding where to find academic information in the CSAS portal.",
            SupportTicketStatus.Open
        )
    };

        foreach (var item in supportTicketData)
        {
            var student =
                students[item.Item2 % students.Count];

            var ticket =
                await db.SupportTickets
                    .FirstOrDefaultAsync(
                        t => t.TicketNumber == item.Item1);

            if (ticket == null)
            {
                db.SupportTickets.Add(
                    new SupportTicket
                    {
                        TicketNumber = item.Item1,
                        StudentId = student.Id,
                        Subject = item.Item3,
                        Category = item.Item4,
                        Description = item.Item5,
                        Status = item.Item6,
                        CreatedAt =
                            now.AddDays(-(item.Item2 + 1)),
                        UpdatedAt =
                            item.Item6 == SupportTicketStatus.Open
                                ? null
                                : now.AddDays(-item.Item2)
                    });
            }
            else
            {
                ticket.StudentId = student.Id;
                ticket.Subject = item.Item3;
                ticket.Category = item.Item4;
                ticket.Description = item.Item5;
                ticket.Status = item.Item6;
                ticket.UpdatedAt =
                    item.Item6 == SupportTicketStatus.Open
                        ? null
                        : now.AddDays(-item.Item2);
            }
        }

        await db.SaveChangesAsync();
    }


}
