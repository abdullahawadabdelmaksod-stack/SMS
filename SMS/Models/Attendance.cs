namespace SMS.Models
{
    /// <summary>
    /// Records a student's attendance for a specific course on a given date.
    /// Student → Attendance is one-to-many; Course → Attendance is one-to-many.
    /// </summary>
    public class Attendance : AuditableEntity
    {
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public string Notes { get; set; } = string.Empty;

        // ── FK → Student ─────────────────────────────────────────────────────
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        // ── FK → Course ──────────────────────────────────────────────────────
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
