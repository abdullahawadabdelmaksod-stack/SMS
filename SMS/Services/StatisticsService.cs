using Microsoft.EntityFrameworkCore;
using SMS.Data;

namespace SMS.Services
{
    /// <summary>
    /// Provides pre-computed statistics for the Dashboard:
    /// dept distribution, level GPA, at-risk counts, overall KPIs.
    /// All queries are optimised with single DB round-trips where possible.
    /// </summary>
    public class StatisticsService
    {
        // ── KPI snapshot (one DB call) ────────────────────────────────────────
        public DashboardKpi GetDashboardKpi()
        {
            using var db = new AppDbContext();

            int totalStudents = db.Students.Count();
            int totalCourses  = db.Courses.Count();
            int totalGrades   = db.Grades.Count();

            double attRate = db.Attendances.Any()
                ? db.Attendances.Count(a => a.IsPresent) * 100.0 / db.Attendances.Count()
                : 0.0;

            double avgScore = db.Grades.Any()
                ? db.Grades.Average(g => g.Score)
                : 0.0;

            return new DashboardKpi(totalStudents, totalCourses, totalGrades, attRate, avgScore);
        }

        // ── Students per department (pie chart data) ──────────────────────────
        public Dictionary<string, double> GetStudentsByDepartment()
        {
            using var db = new AppDbContext();
            return db.Students
                .GroupBy(s => s.Department ?? "Unspecified")
                .Select(g => new { Name = g.Key, Count = (double)g.Count() })
                .ToDictionary(x => x.Name, x => x.Count);
        }

        // ── Average GPA per level (bar chart data) ────────────────────────────
        public Dictionary<string, double> GetAvgGpaByLevel()
        {
            using var db = new AppDbContext();
            return db.Students
                .Include(s => s.Grades)
                .Where(s => s.Grades.Any())
                .AsEnumerable()
                .GroupBy(s => s.Level ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(s => s.Grades.Average(gr =>
                        gr.Score >= 90 ? 4.0 :
                        gr.Score >= 80 ? 3.0 :
                        gr.Score >= 70 ? 2.0 :
                        gr.Score >= 60 ? 1.0 : 0.0)));
        }

        // ── Count of at-risk students (attendance < threshold) ────────────────
        public int GetAtRiskCount(double threshold = 75.0)
        {
            using var db = new AppDbContext();
            return db.Students
                .Include(s => s.Attendances)
                .Where(s => s.Attendances.Any())
                .AsEnumerable()
                .Count(s => s.Attendances.Count(a => a.IsPresent) * 100.0
                            / s.Attendances.Count() < threshold);
        }

        // ── Grade letter distribution (A/B/C/D/F counts) ─────────────────────
        public Dictionary<string, int> GetGradeDistribution()
        {
            using var db = new AppDbContext();
            var dist = db.Grades
                .GroupBy(g => g.LetterGrade)
                .Select(g => new { Letter = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Letter, x => x.Count);

            // Ensure all letters present even if count = 0
            foreach (var l in new[] { "A", "B", "C", "D", "F" })
                dist.TryAdd(l, 0);

            return dist;
        }

        // ── Course enrolment counts (most popular first) ──────────────────────
        public List<CourseEnrolment> GetCourseEnrolments()
        {
            using var db = new AppDbContext();
            return db.Courses
                .Select(c => new CourseEnrolment
                {
                    CourseName = c.Name,
                    CourseCode = c.Code,
                    Count      = c.Grades.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();
        }
    }

    // ── DTOs ─────────────────────────────────────────────────────────────────
    public record DashboardKpi(
        int    TotalStudents,
        int    TotalCourses,
        int    TotalGrades,
        double AttendanceRate,
        double AverageScore);

    public record CourseEnrolment
    {
        public string CourseName { get; init; } = "";
        public string CourseCode { get; init; } = "";
        public int    Count      { get; init; }
    }
}
