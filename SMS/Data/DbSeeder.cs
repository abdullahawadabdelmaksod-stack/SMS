using SMS.Models;


namespace SMS.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (db.Students.Any()) return; // Database already seeded

            // ── Seed Admin User ──────────────────────────────────────────────────
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Username = "admin", Password = "123", Role = "Admin" });
            }

            var courses = new List<Course>
            {
                new Course { CourseId = 1, Name = "Data Structures", Code = "CS201", Credits = 3, Description = "Core algorithms" },
                new Course { CourseId = 2, Name = "Database Systems", Code = "CS301", Credits = 4, Description = "SQL and NoSQL" },
                new Course { CourseId = 3, Name = "Software Engineering", Code = "SE305", Credits = 3, Description = "SDLC & patterns" },
                new Course { CourseId = 4, Name = "Web Development", Code = "WD101", Credits = 3, Description = "Frontend & backend" },
                new Course { CourseId = 5, Name = "Calculus I", Code = "MATH101", Credits = 4, Description = "Limits and derivatives" },
                new Course { CourseId = 6, Name = "Linear Algebra", Code = "MATH201", Credits = 3, Description = "Matrices and vectors" },
                new Course { CourseId = 7, Name = "Physics I", Code = "PHYS101", Credits = 4, Description = "Mechanics" },
                new Course { CourseId = 8, Name = "Computer Architecture", Code = "CS202", Credits = 3, Description = "Hardware fundamentals" },
                new Course { CourseId = 9, Name = "Operating Systems", Code = "CS302", Credits = 4, Description = "Processes and memory" },
                new Course { CourseId = 10, Name = "Computer Networks", Code = "CS303", Credits = 3, Description = "Protocols and sockets" },
                new Course { CourseId = 11, Name = "Artificial Intelligence", Code = "AI301", Credits = 3, Description = "ML fundamentals" },
                new Course { CourseId = 12, Name = "Cybersecurity", Code = "SEC201", Credits = 3, Description = "Network security" }
            };


            var firstNames = new[] { "Ahmed", "Mahmoud", "Mohamed", "Hassan", "Kareem", "Fatima", "Aisha", "Nour", "Yasmin", "Heba", "Khaled", "Omar" };
            var lastNames = new[] { "Ali", "Ibrahim", "Tariq", "Mansour", "El-Sayed", "Farouk", "Mustafa", "Salem" };
            var depts = new[] { "Computer Science", "Information Systems", "Software Engineering", "AI" };
            var levels = new[] { "Freshman", "Sophomore", "Junior", "Senior" };

            var rand = new Random();
            var students = new List<Student>();

            for (int i = 1; i <= 100; i++)
            {
                string first = firstNames[rand.Next(firstNames.Length)];
                string last = lastNames[rand.Next(lastNames.Length)];
                string dept = depts[rand.Next(depts.Length)];
                string lvl = levels[rand.Next(levels.Length)];

                var student = new Student
                {
                    Name = $"{first} {last}",
                    Age = rand.Next(18, 24),
                    Department = dept,
                    Phone = $"01{rand.Next(0, 3)}{rand.Next(10000000, 99999999)}",
                    BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-rand.Next(18, 24)).AddDays(-rand.Next(0, 365))),
                    Level = lvl
                };

                students.Add(student);
            }
            db.Students.AddRange(students);
            db.Courses.AddRange(courses);
            await db.SaveChangesAsync();

            // Get courses from DB to avoid tracking issues
            var courseList = db.Courses.ToList();

            // Add some grades and attendance
            foreach (var student in students)
            {
                int crsCount = rand.Next(1, 5);
                for (int c = 0; c < crsCount; c++)
                {
                    var course = courseList[rand.Next(courseList.Count)];
                    var score = rand.Next(50, 100);
                    var letterGrade = score >= 90 ? "A" : score >= 80 ? "B" : score >= 70 ? "C" : score >= 60 ? "D" : "F";

                    db.Grades.Add(new Grade
                    {
                        StudentId = student.StudentId,
                        CourseId = course.CourseId,
                        Score = score,
                        LetterGrade = letterGrade,
                        Semester = "Fall 2026"
                    });

                    db.Attendances.Add(new Attendance
                    {
                        StudentId = student.StudentId,
                        CourseId = course.CourseId,
                        Date = DateTime.Today.AddDays(-rand.Next(1, 30)),
                        IsPresent = rand.NextDouble() > 0.1
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
