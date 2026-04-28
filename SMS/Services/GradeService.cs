using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;

namespace SMS.Services
{
    /// <summary>
    /// Centralises all grade-related business logic:
    /// GPA calculation, letter-grade mapping, per-student reports.
    /// </summary>
    public class GradeService
    {
        // ── Letter grade → 4.0-scale GPA point ───────────────────────────────
        public static double ScoreToGpaPoint(double score) =>
            score >= 90 ? 4.0 :
            score >= 80 ? 3.0 :
            score >= 70 ? 2.0 :
            score >= 60 ? 1.0 : 0.0;

        public static string ScoreToLetter(double score) =>
            score >= 90 ? "A" :
            score >= 80 ? "B" :
            score >= 70 ? "C" :
            score >= 60 ? "D" : "F";

        // ── GPA for one student ───────────────────────────────────────────────
        public double GetGPA(int studentId)
        {
            using var db = new AppDbContext();
            var grades = db.Grades.Where(g => g.StudentId == studentId).ToList();
            return grades.Any() ? grades.Average(g => ScoreToGpaPoint(g.Score)) : 0.0;
        }

        // ── All grades for one student (with course name) ─────────────────────
        public List<GradeSummary> GetStudentGrades(int studentId)
        {
            using var db = new AppDbContext();
            return db.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Semester)
                .Select(g => new GradeSummary
                {
                    CourseName  = g.Course.Name,
                    CourseCode  = g.Course.Code,
                    Credits     = g.Course.Credits,
                    Score       = g.Score,
                    LetterGrade = g.LetterGrade,
                    Semester    = g.Semester,
                    GpaPoint    = ScoreToGpaPoint(g.Score)
                })
                .ToList();
        }

        // ── Top N students by GPA (for Dashboard) ────────────────────────────
        public List<TopStudentResult> GetTopStudents(int topN = 10)
        {
            using var db = new AppDbContext();
            return db.Students
                .Include(s => s.Grades)
                .Where(s => s.Grades.Any())
                .AsEnumerable()                        // switch to LINQ-to-Objects for GPA calc
                .Select(s => new TopStudentResult
                {
                    StudentId  = s.StudentId,
                    Name       = s.Name,
                    Department = s.Department,
                    GPA        = s.Grades.Average(g => ScoreToGpaPoint(g.Score))
                })
                .OrderByDescending(x => x.GPA)
                .Take(topN)
                .ToList();
        }

        // ── Average score across all students ─────────────────────────────────
        public double GetOverallAverageScore()
        {
            using var db = new AppDbContext();
            return db.Grades.Any() ? db.Grades.Average(g => g.Score) : 0.0;
        }
    }

    // ── DTOs ─────────────────────────────────────────────────────────────────
    public record GradeSummary
    {
        public string CourseName  { get; init; } = "";
        public string CourseCode  { get; init; } = "";
        public int    Credits     { get; init; }
        public double Score       { get; init; }
        public string LetterGrade { get; init; } = "";
        public string Semester    { get; init; } = "";
        public double GpaPoint    { get; init; }
    }

    public record TopStudentResult
    {
        public int    StudentId  { get; init; }
        public string Name       { get; init; } = "";
        public string Department { get; init; } = "";
        public double GPA        { get; init; }
    }
}
