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
        var classRepresentativeHasher = new PasswordHasher<ClassRepresentative>();
        var administratorHasher = new PasswordHasher<Administrator>();
        var lecturerHasher = new PasswordHasher<Lecturer>();

        // =========================================================
        // STUDENTS — 20 RECORDS
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
            var student = await db.Students
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
                student.StudySession = StudySession.Day;
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
        // CLASS REPRESENTATIVES — 20 RECORDS
        // =========================================================

        // (RegNo, FullName, Email, Phone, Nationality)
        var classRepresentativeData = new[]
        {
            ("CP001", "Bob Nkurunziza",       "bob@unilak.ac.rw",       "+250 788 100 001", "Rwandan"),
            ("CP002", "Clarisse Uwimana",     "clarisse@unilak.ac.rw",  "+250 788 100 002", "Rwandan"),
            ("CP003", "Derrick Niyonsenga",   "derrick@unilak.ac.rw",   "+250 788 100 003", "Rwandan"),
            ("CP004", "Emmanuel Habimana",    "emmanuel@unilak.ac.rw",  "+250 788 100 004", "Rwandan"),
            ("CP005", "Florence Mukamana",    "florence@unilak.ac.rw",  "+250 788 100 005", "Rwandan"),
            ("CP006", "Gaspard Bizimana",     "gaspard@unilak.ac.rw",   "+250 788 100 006", "Rwandan"),
            ("CP007", "Hope Uwamahoro",       "hope@unilak.ac.rw",      "+250 788 100 007", "Rwandan"),
            ("CP008", "Ivan Nshimiyimana",    "ivan@unilak.ac.rw",      "+250 788 100 008", "Rwandan"),
            ("CP009", "Josiane Ingabire",     "josiane@unilak.ac.rw",   "+250 788 100 009", "Rwandan"),
            ("CP010", "Kenneth Mugisha",      "kenneth@unilak.ac.rw",   "+250 788 100 010", "Ugandan"),
            ("CP011", "Lydia Uwase",          "lydia@unilak.ac.rw",     "+250 788 100 011", "Rwandan"),
            ("CP012", "Michel Ndayisenga",    "michel@unilak.ac.rw",    "+250 788 100 012", "Rwandan"),
            ("CP013", "Nathalie Mukeshimana", "nathalie@unilak.ac.rw",  "+250 788 100 013", "Rwandan"),
            ("CP014", "Oscar Tuyisenge",      "oscar@unilak.ac.rw",     "+250 788 100 014", "Congolese"),
            ("CP015", "Patricia Uwamariya",   "patricia@unilak.ac.rw",  "+250 788 100 015", "Rwandan"),
            ("CP016", "Robert Niyomugabo",    "robert@unilak.ac.rw",    "+250 788 100 016", "Rwandan"),
            ("CP017", "Sandrine Mukamana",    "sandrine@unilak.ac.rw",  "+250 788 100 017", "Burundian"),
            ("CP018", "Theoneste Habimana",   "theoneste@unilak.ac.rw", "+250 788 100 018", "Rwandan"),
            ("CP019", "Valerie Ingabire",     "valerie@unilak.ac.rw",   "+250 788 100 019", "Rwandan"),
            ("CP020", "William Nkurunziza",   "william@unilak.ac.rw",   "+250 788 100 020", "Kenyan"),
        };

        foreach (var item in classRepresentativeData)
        {
            var representative =
                await db.ClassRepresentatives
                    .FirstOrDefaultAsync(
                        c => c.RegNo == item.Item1);

            if (representative == null)
            {
                representative = new ClassRepresentative
                {
                    RegNo        = item.Item1,
                    FullName     = item.Item2,
                    Email        = item.Item3,
                    PhoneNumber  = item.Item4,
                    Nationality  = item.Item5,
                    Department   = "CIS",
                    StudySession = StudySession.Day,
                    IsActive     = true
                };

                representative.PasswordHash =
                    classRepresentativeHasher.HashPassword(
                        representative,
                        defaultPassword);

                db.ClassRepresentatives.Add(representative);
            }
            else
            {
                representative.FullName     = item.Item2;
                representative.Email        = item.Item3;
                representative.PhoneNumber  = item.Item4;
                representative.Nationality  = item.Item5;
                representative.Department   = "CIS";
                representative.StudySession = StudySession.Day;
                representative.IsActive     = true;

                if (string.IsNullOrWhiteSpace(representative.PasswordHash))
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
        // ADMINISTRATORS — 20 RECORDS
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
        // LECTURERS — 20 RECORDS
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

        // =========================================================
        // ROOMS — 20 RECORDS
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
            var room = await db.Rooms
                .FirstOrDefaultAsync(
                    r => r.RoomNumber == item.Item1);

            if (room == null)
            {
                db.Rooms.Add(new Room
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

        // =========================================================
        // GET LECTURERS FOR COURSE SEEDING
        // =========================================================

        var lecturers =
            await db.Lecturers
                .OrderBy(l => l.Id)
                .Take(20)
                .ToListAsync();

        // =========================================================
        // COURSES — 20 RECORDS
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

        var registrationOpen = DateTime.UtcNow.AddDays(-1);
        var registrationClose = registrationOpen.AddDays(2);
        var courseStart = DateTime.UtcNow.AddDays(-10);
        var courseEnd = DateTime.UtcNow.AddDays(90);

        for (var i = 0; i < courseData.Length; i++)
        {
            var item = courseData[i];

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
                    StudySession = StudySession.Day,
                    LecturerId = lecturers[i % lecturers.Count].Id,
                    StartDate = courseStart,
                    EndDate = courseEnd,
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
                course.StudySession = StudySession.Day;
                course.LecturerId = lecturers[i % lecturers.Count].Id;
                course.StartDate = courseStart;
                course.EndDate = courseEnd;
                course.RegistrationOpenDate = registrationOpen;
                course.RegistrationCloseDate = registrationClose;
                course.IsActive = true;
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // GET COURSES AND ROOMS
        // =========================================================

        var courses =
            await db.Courses
                .OrderBy(c => c.Id)
                .Take(20)
                .ToListAsync();

        var rooms =
            await db.Rooms
                .OrderBy(r => r.Id)
                .Take(20)
                .ToListAsync();

        // =========================================================
        // SCHEDULE ENTRIES — 20 RECORDS
        // =========================================================

        var scheduleSlots = new[]
        {
        (DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
        (DayOfWeek.Monday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

        (DayOfWeek.Tuesday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Tuesday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
        (DayOfWeek.Tuesday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

        (DayOfWeek.Wednesday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (DayOfWeek.Wednesday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Wednesday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
        (DayOfWeek.Wednesday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

        (DayOfWeek.Thursday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Thursday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
        (DayOfWeek.Thursday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

        (DayOfWeek.Friday, new TimeOnly(8, 0), new TimeOnly(10, 0)),
        (DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Friday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
        (DayOfWeek.Friday, new TimeOnly(15, 0), new TimeOnly(17, 0))
    };

        var existingScheduleKeys =
            await db.ScheduleEntries
                .Select(s => new
                {
                    s.CourseId,
                    s.DayOfWeek,
                    s.StartTime
                })
                .ToListAsync();

        if (existingScheduleKeys.Count < 20)
        {
            for (var i = 0; i < 20; i++)
            {
                var course = courses[i % courses.Count];
                var lecturer = lecturers[i % lecturers.Count];
                var room = rooms[i % rooms.Count];
                var slot = scheduleSlots[i];

                var exists =
                    existingScheduleKeys.Any(s =>
                        s.CourseId == course.Id &&
                        s.DayOfWeek == slot.Item1 &&
                        s.StartTime == slot.Item2);

                if (!exists)
                {
                    db.ScheduleEntries.Add(
                        new ScheduleEntry
                        {
                            CourseId = course.Id,
                            LecturerId = lecturer.Id,
                            RoomId = room.Id,
                            DayOfWeek = slot.Item1,
                            StartTime = slot.Item2,
                            EndTime = slot.Item3,
                            StudySession = StudySession.Day,
                            Status = ScheduleStatus.Active,
                            Notes = $"Regular {course.Code} class",
                            IsActive = true
                        });
                }
            }

            await db.SaveChangesAsync();
        }

        // =========================================================
        // ADD 2 EXTRA CLASSES FOR TODAY — CLASSROOM TESTING
        // RWANDA TIME (UTC+2)
        // =========================================================

        DateTime testingNow;

        try
        {
            var rwandaTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "South Africa Standard Time");

            testingNow =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    rwandaTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            testingNow = DateTime.UtcNow.AddHours(2);
        }
        catch (InvalidTimeZoneException)
        {
            testingNow = DateTime.UtcNow.AddHours(2);
        }

        var testingToday = testingNow.DayOfWeek;
        var currentTime = TimeOnly.FromDateTime(testingNow);

        var firstTestingCourse = courses[0];
        var secondTestingCourse = courses[1];

        var firstTestingLecturer = lecturers[0];
        var secondTestingLecturer = lecturers[1];

        var firstTestingRoom = rooms[0];
        var secondTestingRoom = rooms[1];

        var ongoingStart = currentTime.AddMinutes(-30);
        var ongoingEnd = currentTime.AddMinutes(30);

        var upcomingStart = currentTime.AddMinutes(45);
        var upcomingEnd = currentTime.AddMinutes(105);

        var existingOngoingTest =
            await db.ScheduleEntries
                .FirstOrDefaultAsync(s =>
                    s.Notes == "CLASSROOM TEST — ONGOING");

        if (existingOngoingTest == null)
        {
            existingOngoingTest = new ScheduleEntry
            {
                CourseId = firstTestingCourse.Id,
                LecturerId = firstTestingLecturer.Id,
                RoomId = firstTestingRoom.Id,
                DayOfWeek = testingToday,
                StartTime = ongoingStart,
                EndTime = ongoingEnd,
                StudySession = StudySession.Day,
                Status = ScheduleStatus.Active,
                Notes = "CLASSROOM TEST — ONGOING",
                IsActive = true
            };

            db.ScheduleEntries.Add(existingOngoingTest);
        }
        else
        {
            existingOngoingTest.CourseId = firstTestingCourse.Id;
            existingOngoingTest.LecturerId = firstTestingLecturer.Id;
            existingOngoingTest.RoomId = firstTestingRoom.Id;
            existingOngoingTest.DayOfWeek = testingToday;
            existingOngoingTest.StartTime = ongoingStart;
            existingOngoingTest.EndTime = ongoingEnd;
            existingOngoingTest.StudySession = StudySession.Day;
            existingOngoingTest.Status = ScheduleStatus.Active;
            existingOngoingTest.Notes = "CLASSROOM TEST — ONGOING";
            existingOngoingTest.IsActive = true;
        }

        var existingUpcomingTest =
            await db.ScheduleEntries
                .FirstOrDefaultAsync(s =>
                    s.Notes == "CLASSROOM TEST — UPCOMING");

        if (existingUpcomingTest == null)
        {
            existingUpcomingTest = new ScheduleEntry
            {
                CourseId = secondTestingCourse.Id,
                LecturerId = secondTestingLecturer.Id,
                RoomId = secondTestingRoom.Id,
                DayOfWeek = testingToday,
                StartTime = upcomingStart,
                EndTime = upcomingEnd,
                StudySession = StudySession.Day,
                Status = ScheduleStatus.Active,
                Notes = "CLASSROOM TEST — UPCOMING",
                IsActive = true
            };

            db.ScheduleEntries.Add(existingUpcomingTest);
        }
        else
        {
            existingUpcomingTest.CourseId = secondTestingCourse.Id;
            existingUpcomingTest.LecturerId = secondTestingLecturer.Id;
            existingUpcomingTest.RoomId = secondTestingRoom.Id;
            existingUpcomingTest.DayOfWeek = testingToday;
            existingUpcomingTest.StartTime = upcomingStart;
            existingUpcomingTest.EndTime = upcomingEnd;
            existingUpcomingTest.StudySession = StudySession.Day;
            existingUpcomingTest.Status = ScheduleStatus.Active;
            existingUpcomingTest.Notes = "CLASSROOM TEST — UPCOMING";
            existingUpcomingTest.IsActive = true;
        }

        await db.SaveChangesAsync();

        // =========================================================
        // ENROLLMENTS — 15 ENROLLED + 5 REJECTED
        // =========================================================

        var students =
            await db.Students
                .OrderBy(s => s.Id)
                .Take(20)
                .ToListAsync();

        courses =
            await db.Courses
                .OrderBy(c => c.Id)
                .Take(20)
                .ToListAsync();

        var existingEnrollmentKeys =
            await db.Enrollments
                .Select(e => new
                {
                    e.StudentId,
                    e.CourseId
                })
                .ToListAsync();

        var enrollmentCount = existingEnrollmentKeys.Count;

        if (enrollmentCount < 20)
        {
            var enrollmentIndex = 0;

            foreach (var student in students)
            {
                for (var courseIndex = 0;
                     courseIndex < courses.Count;
                     courseIndex++)
                {
                    if (enrollmentCount >= 20)
                    {
                        break;
                    }

                    var course = courses[courseIndex];

                    var exists =
                        existingEnrollmentKeys.Any(e =>
                            e.StudentId == student.Id &&
                            e.CourseId == course.Id);

                    if (exists)
                    {
                        continue;
                    }

                    var status =
                        enrollmentIndex < 15
                            ? EnrollmentStatus.Enrolled
                            : EnrollmentStatus.Closed;

                    db.Enrollments.Add(
                        new Enrollment
                        {
                            StudentId = student.Id,
                            CourseId = course.Id,
                            Status = status,
                            RequestedAt =
                                DateTime.UtcNow.AddDays(
                                    -(enrollmentIndex + 1)),
                            ApprovedAt =
                                status == EnrollmentStatus.Enrolled
                                    ? DateTime.UtcNow.AddDays(
                                        -(enrollmentIndex + 1))
                                    : null
                        });

                    existingEnrollmentKeys.Add(
                        new
                        {
                            StudentId = student.Id,
                            CourseId = course.Id
                        });

                    enrollmentCount++;
                    enrollmentIndex++;
                }

                if (enrollmentCount >= 20)
                {
                    break;
                }
            }

            await db.SaveChangesAsync();
        }

        // =========================================================
        // NORMALIZE DEVELOPMENT ENROLLMENT DATA
        // 15 ENROLLED + 5 REJECTED
        // =========================================================

        var developmentEnrollments =
            await db.Enrollments
                .OrderBy(e => e.Id)
                .Take(20)
                .ToListAsync();

        for (var i = 0; i < developmentEnrollments.Count; i++)
        {
            var enrollment = developmentEnrollments[i];

            if (i < 15)
            {
                enrollment.Status =
                    EnrollmentStatus.Enrolled;

                if (!enrollment.ApprovedAt.HasValue)
                {
                    enrollment.ApprovedAt =
                        DateTime.UtcNow;
                }
            }
            else
            {
                enrollment.Status =
                    EnrollmentStatus.Closed;

                enrollment.ApprovedAt = null;
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // ENSURE STU001 HAS 5 ENROLLED COURSES
        // =========================================================

        var firstStudent =
            await db.Students
                .FirstOrDefaultAsync(
                    s => s.RegNo == "STU001");

        if (firstStudent != null)
        {
            var firstFiveCourses =
                await db.Courses
                    .OrderBy(c => c.Id)
                    .Take(5)
                    .ToListAsync();

            foreach (var course in firstFiveCourses)
            {
                var enrollment =
                    await db.Enrollments
                        .FirstOrDefaultAsync(e =>
                            e.StudentId == firstStudent.Id &&
                            e.CourseId == course.Id);

                if (enrollment == null)
                {
                    db.Enrollments.Add(
                        new Enrollment
                        {
                            StudentId = firstStudent.Id,
                            CourseId = course.Id,
                            Status = EnrollmentStatus.Enrolled,
                            RequestedAt =
                                DateTime.UtcNow.AddDays(-1),
                            ApprovedAt =
                                DateTime.UtcNow
                        });
                }
                else
                {
                    enrollment.Status =
                        EnrollmentStatus.Enrolled;

                    if (!enrollment.ApprovedAt.HasValue)
                    {
                        enrollment.ApprovedAt =
                            DateTime.UtcNow;
                    }
                }
            }

            await db.SaveChangesAsync();
        }

        // =========================================================
        // EXAMS — TEST DATA FOR VISUALISATION
        // =========================================================

        var examToday = testingNow.Date;

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
                examToday.AddDays(
                    definition.DaysFromToday);

            var existingExam =
                await db.Exams
                    .FirstOrDefaultAsync(e =>
                        e.CourseId == course.Id &&
                        e.ExamType == definition.ExamType);

            if (existingExam == null)
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
                        CreatedAt = DateTime.UtcNow
                    });
            }
            else
            {
                existingExam.RoomId = room.Id;
                existingExam.ExamDate = examDate;
                existingExam.StartTime = definition.StartTime;
                existingExam.EndTime = definition.EndTime;
                existingExam.IsActive = true;
                existingExam.Notes =
                    $"{definition.ExamType} examination for {course.Code}";
                existingExam.UpdatedAt = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // SUPPORT TICKETS — 8 RECORDS
        // =========================================================

        var supportTicketData = new[]
        {
        (
            TicketNumber: "SUP-A1B2C3",
            StudentRegNo: "STU001",
            Subject: "Cannot access my account",
            Category: SupportCategory.AccountAccess,
            Description: "The student cannot log in using the registered credentials.",
            Status: SupportTicketStatus.Open
        ),
        (
            TicketNumber: "SUP-D4E5F6",
            StudentRegNo: "STU002",
            Subject: "Schedule showing incorrect time",
            Category: SupportCategory.CourseSchedule,
            Description: "A scheduled class appears at the wrong time on the student timetable.",
            Status: SupportTicketStatus.InProgress
        ),
        (
            TicketNumber: "SUP-G7H8I9",
            StudentRegNo: "STU003",
            Subject: "Unable to view course",
            Category: SupportCategory.CourseSchedule,
            Description: "The student cannot see one of the courses assigned to the class.",
            Status: SupportTicketStatus.Resolved
        ),
        (
            TicketNumber: "SUP-J1K2L3",
            StudentRegNo: "STU004",
            Subject: "Examination timetable issue",
            Category: SupportCategory.Examination,
            Description: "The student reported an issue with the examination date displayed.",
            Status: SupportTicketStatus.Closed
        ),
        (
            TicketNumber: "SUP-M4N5O6",
            StudentRegNo: "STU005",
            Subject: "Enrollment information missing",
            Category: SupportCategory.Enrollment,
            Description: "An enrolled course is not appearing correctly in the student's enrollment list.",
            Status: SupportTicketStatus.Open
        ),
        (
            TicketNumber: "SUP-P7Q8R9",
            StudentRegNo: "STU006",
            Subject: "Password reset request",
            Category: SupportCategory.AccountAccess,
            Description: "The student requested assistance with resetting the account password.",
            Status: SupportTicketStatus.InProgress
        ),
        (
            TicketNumber: "SUP-S1T2U3",
            StudentRegNo: "STU007",
            Subject: "System page not loading",
            Category: SupportCategory.TechnicalIssue,
            Description: "The student reported that a system page fails to load correctly.",
            Status: SupportTicketStatus.Resolved
        ),
        (
            TicketNumber: "SUP-V4W5X6",
            StudentRegNo: "STU008",
            Subject: "General system assistance",
            Category: SupportCategory.Other,
            Description: "The student requested general assistance using the academic awareness system.",
            Status: SupportTicketStatus.Closed
        )
    };

        foreach (var item in supportTicketData)
        {
            var student =
                await db.Students
                    .FirstOrDefaultAsync(
                        s => s.RegNo == item.StudentRegNo);

            if (student == null)
            {
                continue;
            }

            var ticket =
                await db.SupportTickets
                    .FirstOrDefaultAsync(
                        t => t.TicketNumber == item.TicketNumber);

            if (ticket == null)
            {
                ticket = new SupportTicket
                {
                    TicketNumber = item.TicketNumber,
                    StudentId = student.Id,
                    Subject = item.Subject,
                    Category = item.Category,
                    Description = item.Description,
                    Status = item.Status,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    UpdatedAt = DateTime.UtcNow
                };

                db.SupportTickets.Add(ticket);
            }
            else
            {
                ticket.StudentId = student.Id;
                ticket.Subject = item.Subject;
                ticket.Category = item.Category;
                ticket.Description = item.Description;
                ticket.Status = item.Status;
                ticket.UpdatedAt = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync();
    }


}
