using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;

namespace SMS.Services
{
    /// <summary>
    /// Centralises all attendance-related business logic:
    /// rate calculation, at-risk detection, bulk marking.
    /// </summary>
    public class AttendanceService
    {
        // ── Overall attendance rate for a student (all courses) ───────────────
        public double GetAttendanceRate(int studentId)
        {
            using var db = new AppDbContext();
            var records = db.Attendances.Where(a => a.StudentId == studentId).ToList();
            if (!records.Any()) return 0.0;
            return records.Count(a => a.IsPresent) * 100.0 / records.Count;
        }

        // ── Attendance rate for a student in one specific course ──────────────
        public double GetAttendanceRate(int studentId, int courseId)
        {
            using var db = new AppDbContext();
            var records = db.Attendances
                .Where(a => a.StudentId == studentId && a.CourseId == courseId)
                .ToList();
            if (!records.Any()) return 0.0;
            return records.Count(a => a.IsPresent) * 100.0 / records.Count;
        }

        // ── Detailed breakdown per course ────────────────────────────────────
        public List<AttendanceSummary> GetStudentAttendance(int studentId)
        {
            using var db = new AppDbContext();
            return db.Attendances
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .GroupBy(a => new { a.CourseId, a.Course.Name, a.Course.Code })
                .Select(g => new AttendanceSummary
                {
                    CourseId   = g.Key.CourseId,
                    CourseName = g.Key.Name,
                    CourseCode = g.Key.Code,
                    Total      = g.Count(),
                    Present    = g.Count(a => a.IsPresent),
                    Absent     = g.Count(a => !a.IsPresent),
                    Rate       = g.Count(a => a.IsPresent) * 100.0 / g.Count()
                })
                .OrderByDescending(x => x.Rate)
                .ToList();
        }

        // ── Students below a given attendance threshold ───────────────────────
        public List<AtRiskStudent> GetAtRiskStudents(double threshold = 75.0)
        {
            using var db = new AppDbContext();
            return db.Students
                .Include(s => s.Attendances)
                .Where(s => s.Attendances.Any())
                .AsEnumerable()
                .Select(s => new AtRiskStudent
                {
                    StudentId      = s.StudentId,
                    Name           = s.Name,
                    Department     = s.Department,
                    AttendanceRate = s.Attendances.Count(a => a.IsPresent) * 100.0
                                    / s.Attendances.Count()
                })
                .Where(x => x.AttendanceRate < threshold)
                .OrderBy(x => x.AttendanceRate)
                .ToList();
        }

        // ── Bulk-mark a session (all students present unless excluded) ────────
        public void BulkMarkAttendance(int courseId, DateTime date,
                                       List<int> presentStudentIds,
                                       List<int> allStudentIds)
        {
            using var db = new AppDbContext();
            foreach (var sid in allStudentIds)
            {
                db.Attendances.Add(new Attendance
                {
                    StudentId = sid,
                    CourseId  = courseId,
                    Date      = date,
                    IsPresent = presentStudentIds.Contains(sid),
                    Notes     = presentStudentIds.Contains(sid) ? "" : "Absent (bulk mark)"
                });
            }
            db.SaveChanges();
        }

        // ── Overall attendance rate across ALL students ────────────────────────
        public double GetOverallAttendanceRate()
        {
            using var db = new AppDbContext();
            if (!db.Attendances.Any()) return 0.0;
            int total   = db.Attendances.Count();
            int present = db.Attendances.Count(a => a.IsPresent);
            return present * 100.0 / total;
        }
    }

    // ── DTOs ─────────────────────────────────────────────────────────────────
    public record AttendanceSummary
    {
        public int    CourseId   { get; init; }
        public string CourseName { get; init; } = "";
        public string CourseCode { get; init; } = "";
        public int    Total      { get; init; }
        public int    Present    { get; init; }
        public int    Absent     { get; init; }
        public double Rate       { get; init; }
    }

    public record AtRiskStudent
    {
        public int    StudentId      { get; init; }
        public string Name           { get; init; } = "";
        public string Department     { get; init; } = "";
        public double AttendanceRate { get; init; }
    }
}
