using SMS.Models;

namespace SMS.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Guards will be applied per-entity to ensure updates work correctly

            var rand = new Random(42); // fixed seed for reproducibility

            // ── Professors ───────────────────────────────────────────────────
            var profNames = new[] { "Dr. Ahmed Hassan", "Dr. Mahmoud Ali", "Dr. Fatima Ibrahim", "Dr. Tarek Mansour", "Dr. Mona El-Sayed" };
            var profDepts = new[] { "Computer Science", "Mathematics", "Physics", "Engineering", "Information Systems" };
            
            var profs = new List<Professor>();
            for (int i = 0; i < profNames.Length; i++)
            {
                profs.Add(new Professor
                {
                    Name = profNames[i],
                    Email = profNames[i].Replace("Dr. ", "").Replace("-", "").Replace(" ", ".").ToLower() + "@university.edu.eg",
                    Department = profDepts[i],
                    Phone = $"010{rand.Next(10000000, 99999999)}"
                });
            }
            if (!db.Professors.Any())
            {
                db.Professors.AddRange(profs);
                await db.SaveChangesAsync();
            }
            else
            {
                profs = db.Professors.ToList();
            }

            // ── Users ────────────────────────────────────────────────────────
            if (!db.Users.Any(u => u.Username == "admin"))
                db.Users.Add(new User { Username = "admin", Password = "123", Role = "Admin" });

            if (!db.Users.Any(u => u.Username == "parent1"))
                db.Users.Add(new User { Username = "parent1", Password = "par1", Role = "Parent" });
            
            // Automatically generate a User login for every Professor
            foreach (var p in profs)
            {
                string username = p.Email.Split('@')[0]; // e.g. ahmed.hassan
                if (!db.Users.Any(u => u.Username == username))
                {
                    db.Users.Add(new User 
                    { 
                        Username = username, 
                        Password = "123", // Default password for all professors
                        Role = "Professor" 
                    });
                }
            }
            await db.SaveChangesAsync();

            // ── Courses ──────────────────────────────────────────────────────
            var courseList = db.Courses.ToList();

            if (!courseList.Any())
            {
                var courses = new List<Course>
                {
                    new() { Name = "Data Structures",       Code = "CS201",   Credits = 3, Description = "Core algorithms and data structures" },
                    new() { Name = "Database Systems",      Code = "CS301",   Credits = 4, Description = "Relational and NoSQL databases" },
                    new() { Name = "Software Engineering",  Code = "SE305",   Credits = 3, Description = "SDLC, design patterns, Agile" },
                    new() { Name = "Web Development",       Code = "WD101",   Credits = 3, Description = "HTML, CSS, JS and backend APIs" },
                    new() { Name = "Calculus I",            Code = "MATH101", Credits = 4, Description = "Limits, derivatives, and integrals" },
                    new() { Name = "Linear Algebra",        Code = "MATH201", Credits = 3, Description = "Matrices, vectors, and eigenvalues" },
                    new() { Name = "Physics I",             Code = "PHYS101", Credits = 4, Description = "Classical mechanics and kinematics" },
                    new() { Name = "Computer Architecture", Code = "CS202",   Credits = 3, Description = "CPU design and memory hierarchy" },
                    new() { Name = "Operating Systems",     Code = "CS302",   Credits = 4, Description = "Processes, threads, and memory management" },
                    new() { Name = "Computer Networks",     Code = "CS303",   Credits = 3, Description = "Protocols, sockets, and network security" },
                    new() { Name = "Artificial Intelligence", Code = "AI301", Credits = 3, Description = "Search, ML fundamentals, neural networks" },
                    new() { Name = "Cybersecurity",         Code = "SEC201",  Credits = 3, Description = "Encryption, attacks, and defences" }
                };
                
                foreach(var c in courses)
                {
                    c.ProfessorId = GetProfessorForCourse(c.Code, profs);
                }
                db.Courses.AddRange(courses);
                await db.SaveChangesAsync();
                courseList = db.Courses.ToList();
            }
            else
            {
                bool coursesUpdated = false;
                foreach(var c in courseList)
                {
                    // Force update to correct professor department
                    int correctProfId = GetProfessorForCourse(c.Code, profs);
                    if(c.ProfessorId != correctProfId)
                    {
                        c.ProfessorId = correctProfId;
                        coursesUpdated = true;
                    }
                }
                if (coursesUpdated) await db.SaveChangesAsync();
            }

            // ── Guard: only seed students if empty ───────────────────────────
            if (db.Students.Any()) return;

            // ── Students ─────────────────────────────────────────────────────
            var firstNames = new[] { "Ahmed", "Mahmoud", "Mohamed", "Hassan", "Kareem",
                                     "Fatima", "Aisha", "Nour", "Yasmin", "Heba",
                                     "Khaled", "Omar", "Sara", "Layla", "Rania",
                                     "Tarek", "Mostafa", "Dina", "Mona", "Ziad" };
            var lastNames  = new[] { "Ali", "Ibrahim", "Tariq", "Mansour", "El-Sayed",
                                     "Farouk", "Mustafa", "Salem", "Hassan", "Nasser" };
            var depts  = new[] { "AI", "CS", "IS", "SE" };
            var levels = new[] { "Freshman", "Sophomore", "Junior", "Senior" };

            var students = new List<Student>();

            for (int i = 1; i <= 100; i++)
            {
                students.Add(new Student
                {
                    Name       = $"{firstNames[rand.Next(firstNames.Length)]} {lastNames[rand.Next(lastNames.Length)]}",
                    Department = depts[rand.Next(depts.Length)],
                    Phone      = $"01{rand.Next(0, 3)}{rand.Next(10_000_000, 99_999_999)}",
                    BirthDate  = DateOnly.FromDateTime(
                        DateTime.Today.AddYears(-rand.Next(18, 25)).AddDays(-rand.Next(0, 365))),
                    Level = levels[rand.Next(levels.Length)]
                });
            }
            db.Students.AddRange(students);
            await db.SaveChangesAsync(); // Get auto-generated StudentIds

            // ── Semesters ────────────────────────────────────────────────────
            var semesters = new[] { "Fall 2024", "Spring 2025", "Fall 2025", "Spring 2026" };

            var grades     = new List<Grade>();
            var attendances = new List<Attendance>();

            foreach (var student in students)
            {
                // Each student enrolled in 4–7 distinct courses
                int enrolCount   = rand.Next(4, 8);
                var enrolled     = courseList.OrderBy(_ => rand.Next()).Take(enrolCount).ToList();
                string semester  = semesters[rand.Next(semesters.Length)];

                foreach (var course in enrolled)
                {
                    // ── Grade ─────────────────────────────────────────────────
                    double score = Math.Round(rand.Next(45, 101) + rand.NextDouble(), 1);
                    string letter = score >= 90 ? "A" : score >= 80 ? "B" : score >= 70 ? "C" : score >= 60 ? "D" : "F";

                    grades.Add(new Grade
                    {
                        StudentId   = student.StudentId,
                        CourseId    = course.CourseId,
                        Score       = score,
                        LetterGrade = letter,
                        Semester    = semester
                    });

                    // ── Attendance: 20–35 session records per enrolment ───────
                    int sessions = rand.Next(20, 36);
                    for (int d = 0; d < sessions; d++)
                    {
                        // Sessions spread over the last 120 days, every ~3 days
                        var sessionDate = DateTime.Today.AddDays(-(sessions - d) * 3 - rand.Next(0, 2));
                        bool present    = rand.NextDouble() > 0.12; // ~88 % attendance rate

                        attendances.Add(new Attendance
                        {
                            StudentId = student.StudentId,
                            CourseId  = course.CourseId,
                            Date      = sessionDate,
                            IsPresent = present,
                            Notes     = !present && rand.NextDouble() > 0.5
                                        ? rand.Next(3) switch
                                          {
                                              0 => "Medical leave",
                                              1 => "Family emergency",
                                              _ => "No reason given"
                                          }
                                        : string.Empty
                        });
                    }
                }
            }

            // Batch insert for performance
            db.Grades.AddRange(grades);
            db.Attendances.AddRange(attendances);
            await db.SaveChangesAsync();
        }

        // ── Helper Method for Department Mapping ─────────────────────────────
        private static int GetProfessorForCourse(string courseCode, List<Professor> profs)
        {
            string targetDept = "CS"; // Default fallback
            
            if (courseCode.StartsWith("MATH") || courseCode.StartsWith("PHYS")) targetDept = "CS"; // Math/Physics fallback to CS
            else if (courseCode.StartsWith("SE")) targetDept = "SE";
            else if (courseCode.StartsWith("WD") || courseCode.StartsWith("SEC")) targetDept = "IS";
            else if (courseCode.StartsWith("CS")) targetDept = "CS";
            else if (courseCode.StartsWith("AI")) targetDept = "AI";

            // Attempt to find exact department match, fallback to IS, then fallback to first available
            var prof = profs.FirstOrDefault(p => p.Department != null && p.Department.ToUpper() == targetDept) 
                       ?? profs.FirstOrDefault(p => p.Department != null && p.Department.ToUpper() == "IS")
                       ?? profs.First();
                       
            return prof.ProfessorId;
        }
    }
}
