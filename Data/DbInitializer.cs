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
        var administratorHasher =
            new PasswordHasher<Administrator>();
        var lecturerHasher =
            new PasswordHasher<Lecturer>();

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
            else if (string.IsNullOrWhiteSpace(student.PasswordHash))
            {
                student.PasswordHash =
                    studentHasher.HashPassword(
                        student,
                        defaultPassword);
            }
        }

        await db.SaveChangesAsync();

        // =========================================================
        // CLASS REPRESENTATIVES — 20 RECORDS
        // =========================================================

        var classRepresentativeData = new[]
        {
            ("CP001", "Bob Nkurunziza", "bob@unilak.ac.rw"),
            ("CP002", "Clarisse Uwimana", "clarisse@unilak.ac.rw"),
            ("CP003", "Derrick Niyonsenga", "derrick@unilak.ac.rw"),
            ("CP004", "Emmanuel Habimana", "emmanuel@unilak.ac.rw"),
            ("CP005", "Florence Mukamana", "florence@unilak.ac.rw"),
            ("CP006", "Gaspard Bizimana", "gaspard@unilak.ac.rw"),
            ("CP007", "Hope Uwamahoro", "hope@unilak.ac.rw"),
            ("CP008", "Ivan Nshimiyimana", "ivan@unilak.ac.rw"),
            ("CP009", "Josiane Ingabire", "josiane@unilak.ac.rw"),
            ("CP010", "Kenneth Mugisha", "kenneth@unilak.ac.rw"),
            ("CP011", "Lydia Uwase", "lydia@unilak.ac.rw"),
            ("CP012", "Michel Ndayisenga", "michel@unilak.ac.rw"),
            ("CP013", "Nathalie Mukeshimana", "nathalie@unilak.ac.rw"),
            ("CP014", "Oscar Tuyisenge", "oscar@unilak.ac.rw"),
            ("CP015", "Patricia Uwamariya", "patricia@unilak.ac.rw"),
            ("CP016", "Robert Niyomugabo", "robert@unilak.ac.rw"),
            ("CP017", "Sandrine Mukamana", "sandrine@unilak.ac.rw"),
            ("CP018", "Theoneste Habimana", "theoneste@unilak.ac.rw"),
            ("CP019", "Valerie Ingabire", "valerie@unilak.ac.rw"),
            ("CP020", "William Nkurunziza", "william@unilak.ac.rw")
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
                    RegNo = item.Item1,
                    FullName = item.Item2,
                    Email = item.Item3,
                    Department = "CIS",
                    StudySession = StudySession.Day,
                    IsActive = true
                };

                representative.PasswordHash =
                    classRepresentativeHasher.HashPassword(
                        representative,
                        defaultPassword);

                db.ClassRepresentatives.Add(representative);
            }
            else if (string.IsNullOrWhiteSpace(
                representative.PasswordHash))
            {
                representative.PasswordHash =
                    classRepresentativeHasher.HashPassword(
                        representative,
                        defaultPassword);
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
            else if (string.IsNullOrWhiteSpace(
                administrator.PasswordHash))
            {
                administrator.PasswordHash =
                    administratorHasher.HashPassword(
                        administrator,
                        defaultPassword);
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
            else if (string.IsNullOrWhiteSpace(
                lecturer.PasswordHash))
            {
                lecturer.PasswordHash =
                    lecturerHasher.HashPassword(
                        lecturer,
                        defaultPassword);
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
            if (!await db.Rooms.AnyAsync(
                r => r.RoomNumber == item.Item1))
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

        var existingCourseCodes =
            await db.Courses
                .Select(c => c.Code)
                .ToListAsync();

        for (var i = 0; i < courseData.Length; i++)
        {
            var item = courseData[i];

            if (!existingCourseCodes.Contains(item.Item1))
            {
                db.Courses.Add(new Course
                {
                    Code = item.Item1,
                    Name = item.Item2,
                    Description = item.Item4,
                    Credits = item.Item3,
                    Status = CourseStatus.Available,
                    StudySession = StudySession.Day,
                    LecturerId = lecturers[i % lecturers.Count].Id,
                    StartDate = new DateTime(2026, 9, 1),
                    EndDate = new DateTime(2026, 12, 15),
                    RegistrationOpenDate =
                        new DateTime(2026, 8, 25),
                    RegistrationCloseDate =
                        new DateTime(2026, 9, 30),
                    IsActive = true
                });
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
            (DayOfWeek.Monday,  new TimeOnly(8, 0),  new TimeOnly(10, 0)),
            (DayOfWeek.Monday,  new TimeOnly(10, 0), new TimeOnly(12, 0)),
            (DayOfWeek.Monday,  new TimeOnly(13, 0), new TimeOnly(15, 0)),
            (DayOfWeek.Monday,  new TimeOnly(15, 0), new TimeOnly(17, 0)),

            (DayOfWeek.Tuesday, new TimeOnly(8, 0),  new TimeOnly(10, 0)),
            (DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
            (DayOfWeek.Tuesday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
            (DayOfWeek.Tuesday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

            (DayOfWeek.Wednesday, new TimeOnly(8, 0),  new TimeOnly(10, 0)),
            (DayOfWeek.Wednesday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
            (DayOfWeek.Wednesday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
            (DayOfWeek.Wednesday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

            (DayOfWeek.Thursday, new TimeOnly(8, 0),  new TimeOnly(10, 0)),
            (DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
            (DayOfWeek.Thursday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
            (DayOfWeek.Thursday, new TimeOnly(15, 0), new TimeOnly(17, 0)),

            (DayOfWeek.Friday, new TimeOnly(8, 0),  new TimeOnly(10, 0)),
            (DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(12, 0)),
            (DayOfWeek.Friday, new TimeOnly(13, 0), new TimeOnly(15, 0)),
            (DayOfWeek.Friday, new TimeOnly(15, 0), new TimeOnly(17, 0))
        };

        var existingScheduleCount =
            await db.ScheduleEntries.CountAsync();

        if (existingScheduleCount < 20)
        {
            var existingScheduleKeys =
                await db.ScheduleEntries
                    .Select(s => new
                    {
                        s.CourseId,
                        s.DayOfWeek,
                        s.StartTime
                    })
                    .ToListAsync();

            for (var i = existingScheduleCount;
                 i < 20;
                 i++)
            {
                var course = courses[i % courses.Count];
                var lecturer =
                    lecturers[i % lecturers.Count];
                var room =
                    rooms[i % rooms.Count];
                var slot =
                    scheduleSlots[i];

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
                            Notes =
                                $"Regular {course.Code} class",
                            IsActive = true
                        });
                }
            }

            await db.SaveChangesAsync();
        }

        // =========================================================
        // ENROLLMENTS — 20 RECORDS
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

        var enrollmentCount =
            existingEnrollmentKeys.Count;

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

                    var course =
                        courses[courseIndex];

                    var exists =
                        existingEnrollmentKeys.Any(e =>
                            e.StudentId == student.Id &&
                            e.CourseId == course.Id);

                    if (exists)
                    {
                        continue;
                    }

                    var status =
                        student.RegNo == "STU001" &&
                        courseIndex < 5
                            ? EnrollmentStatus.Enrolled
                            : enrollmentIndex % 3 == 0
                                ? EnrollmentStatus.Enrolled
                                : enrollmentIndex % 3 == 1
                                    ? EnrollmentStatus.Pending
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
                                status ==
                                EnrollmentStatus.Enrolled
                                    ? DateTime.UtcNow.AddDays(
                                        -(enrollmentIndex + 1))
                                    : null
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
        // ENSURE STU001 HAS ENROLLED COURSES
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
                            Status =
                                EnrollmentStatus.Enrolled,
                            RequestedAt =
                                DateTime.UtcNow.AddDays(-7),
                            ApprovedAt =
                                DateTime.UtcNow.AddDays(-6)
                        });
                }
                else
                {
                    enrollment.Status =
                        EnrollmentStatus.Enrolled;

                    if (!enrollment.ApprovedAt.HasValue)
                    {
                        enrollment.ApprovedAt =
                            DateTime.UtcNow.AddDays(-6);
                    }
                }
            }

            await db.SaveChangesAsync();
        }
    }
}