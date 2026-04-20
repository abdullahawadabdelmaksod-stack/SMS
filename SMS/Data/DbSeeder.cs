using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.Data
{
    /// <summary>
    /// Idempotent database seeder.
    /// Call <see cref="SeedAsync"/> once on startup; it is a no-op if data already exists.
    /// </summary>
    public static class DbSeeder
    {
        // ── Egyptian name pools ─────────────────────────────────────────────────
        private static readonly string[] MaleFirst = {
            "Mohamed", "Ahmed", "Omar", "Ali", "Youssef", "Karim", "Tarek", "Basem",
            "Sherif", "Amr", "Mahmoud", "Ibrahim", "Khaled", "Hassan", "Mostafa",
            "Tamer", "Walid", "Samer", "Nabil", "Rami", "Hossam", "Adel", "Samir",
            "Ashraf", "Essam"
        };
        private static readonly string[] FemaleFirst = {
            "Nour", "Sara", "Fatima", "Mariam", "Hana", "Rania", "Dina", "Aya",
            "Laila", "Yasmine", "Mona", "Eman", "Heba", "Amira", "Nadia",
            "Doaa", "Noha", "Reem", "Salma", "Ghada", "Lobna", "Abeer", "Sahar",
            "Nagwa", "Mervat"
        };
        private static readonly string[] LastNames = {
            "El-Sayed", "Hassan", "Ibrahim", "Mohamed", "Ahmed", "Ali", "Omar",
            "Khalil", "Farouk", "Mansour", "Youssef", "Nasser", "Salah", "Zaki",
            "Gaber", "Hamid", "Ramadan", "Shawky", "Barakat", "Amin", "Badawi",
            "Qasem", "Fouad", "Lotfy", "Sherif", "Tawfik", "Morsi", "Atef",
            "Helmy", "Ragab"
        };
        private static readonly string[] Departments = {
            "Computer Science", "Engineering", "Medicine", "Business Administration",
            "Law", "Architecture", "Pharmacy", "Information Technology",
            "Mathematics", "Physics"
        };
        private static readonly string[] Levels  = { "Level 1", "Level 2", "Level 3", "Level 4" };
        private static readonly string[] Cities  = {
            "Cairo", "Alexandria", "Giza", "Luxor", "Aswan", "Tanta", "Mansoura",
            "Zagazig", "Ismailia", "Suez", "Port Said", "Hurghada"
        };
        private static readonly string[] Streets = {
            "Nile St", "Tahrir Sq", "Ramses Ave", "El-Gomhuria St",
            "El-Nasr Rd", "Salah Salem St", "Al-Azhar St", "Ahmed Orabi St"
        };

        // ── Courses ─────────────────────────────────────────────────────────────
        private static readonly (int id, string name, string code, string desc, int cred)[] CourseData =
        {
            (1,  "Introduction to Computer Science", "CS101",  "Fundamentals of computing.",       3),
            (2,  "Data Structures & Algorithms",     "CS201",  "Core data structures.",             3),
            (3,  "Database Systems",                 "CS301",  "Relational DB theory and SQL.",     3),
            (4,  "Calculus I",                       "MATH101","Limits, derivatives, integrals.",   4),
            (5,  "Applied Physics",                  "PHY101", "Mechanics and thermodynamics.",     3),
            (6,  "Business Communication",           "BUS101", "Professional business writing.",    2),
            (7,  "Engineering Drawing",              "ENG101", "Technical drawing principles.",     2),
            (8,  "Pharmaceutical Chemistry",         "PHA101", "Drug synthesis fundamentals.",      3),
            (9,  "Constitutional Law",               "LAW101", "Egyptian constitutional law.",      3),
            (10, "Architectural Design I",           "ARC101", "Principles of design.",             4),
        };

        private static readonly string[] Semesters = { "2024-S1", "2024-S2", "2025-S1" };

        public static async Task SeedAsync(AppDbContext db)
        {
            // Skip if already populated
            if (await db.Students.AnyAsync()) return;

            var rng  = new Random(42);
            var now  = DateTime.UtcNow;

            // ────────────────────────────────────────────────────────────────────
            // 1. Admin user
            // ────────────────────────────────────────────────────────────────────
            if (!await db.Users.AnyAsync())
            {
                db.Users.Add(new User
                {
                    Username = "admin", Password = "admin123",
                    Role = "Admin", CreatedAt = now, UpdatedAt = now
                });
                await db.SaveChangesAsync();
            }

            // ────────────────────────────────────────────────────────────────────
            // 2. Courses
            // ────────────────────────────────────────────────────────────────────
            foreach (var (id, name, code, desc, cred) in CourseData)
            {
                if (!await db.Courses.AnyAsync(c => c.CourseId == id))
                    db.Courses.Add(new Course
                    {
                        CourseId = id, Name = name, Code = code,
                        Description = desc, Credits = cred,
                        CreatedAt = now, UpdatedAt = now
                    });
            }
            await db.SaveChangesAsync();

            // ────────────────────────────────────────────────────────────────────
            // 3. Students (100 records, 50 male + 50 female)
            // ────────────────────────────────────────────────────────────────────
            var students = new List<Student>();
            for (int i = 0; i < 100; i++)
            {
                bool   isMale   = i < 50;
                string firstName = isMale
                    ? MaleFirst [i % MaleFirst .Length]
                    : FemaleFirst[i % FemaleFirst.Length];
                string lastName  = LastNames[i % LastNames.Length];
                int    age       = rng.Next(18, 27);
                var    birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-age)
                                                .AddDays(-rng.Next(0, 365)));
                string phone     = $"010{rng.Next(10000000, 99999999):D8}";
                string city      = Cities [rng.Next(Cities.Length)];
                string street    = Streets[rng.Next(Streets.Length)];
                string address   = $"{rng.Next(1, 200)} {street}, {city}";

                students.Add(new Student
                {
                    Name       = $"{firstName} {lastName}",
                    Age        = age,
                    Department = Departments[i % Departments.Length],
                    Phone      = phone,
                    Address    = address,
                    BirthDate  = birthDate,
                    Level      = Levels[rng.Next(Levels.Length)],
                    CreatedAt  = now,
                    UpdatedAt  = now
                });
            }
            db.Students.AddRange(students);
            await db.SaveChangesAsync();

            // ────────────────────────────────────────────────────────────────────
            // 4. Grades (each student gets 2–4 course grades per semester)
            // ────────────────────────────────────────────────────────────────────
            var allStudentIds = await db.Students.Select(s => s.StudentId).ToListAsync();
            var allCourseIds  = await db.Courses .Select(c => c.CourseId) .ToListAsync();

            var grades = new List<Grade>();
            foreach (var sid in allStudentIds)
            {
                // pick 3 distinct courses per student
                var picked = allCourseIds.OrderBy(_ => rng.Next()).Take(3).ToList();
                foreach (var cid in picked)
                {
                    double score = Math.Round(40 + rng.NextDouble() * 60, 1); // 40–100
                    grades.Add(new Grade
                    {
                        StudentId   = sid,
                        CourseId    = cid,
                        Score       = score,
                        LetterGrade = ScoreToLetter(score),
                        Semester    = Semesters[rng.Next(Semesters.Length)],
                        CreatedAt   = now,
                        UpdatedAt   = now
                    });
                }
            }
            db.Grades.AddRange(grades);
            await db.SaveChangesAsync();

            // ────────────────────────────────────────────────────────────────────
            // 5. Attendance (3 sessions per student per enrolled course)
            // ────────────────────────────────────────────────────────────────────
            var attendances     = new List<Attendance>();
            var attendanceIndex = new HashSet<(int sid, int cid, DateTime date)>();

            foreach (var sid in allStudentIds)
            {
                var picked = allCourseIds.OrderBy(_ => rng.Next()).Take(3).ToList();
                foreach (var cid in picked)
                {
                    for (int d = 0; d < 3; d++)
                    {
                        // 3 Mondays going back from today
                        var date = DateTime.UtcNow.Date.AddDays(-(d + 1) * 7);
                        var key  = (sid, cid, date);
                        if (attendanceIndex.Contains(key)) continue;
                        attendanceIndex.Add(key);

                        attendances.Add(new Attendance
                        {
                            StudentId = sid,
                            CourseId  = cid,
                            Date      = date,
                            IsPresent = rng.NextDouble() > 0.15,   // 85 % present rate
                            Notes     = "",
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                    }
                }
            }
            db.Attendances.AddRange(attendances);
            await db.SaveChangesAsync();
        }

        // ── Helpers ─────────────────────────────────────────────────────────────
        private static string ScoreToLetter(double s) => s switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _     => "F"
        };
    }
}
