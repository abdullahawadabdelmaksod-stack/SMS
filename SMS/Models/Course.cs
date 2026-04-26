namespace SMS.Models
{
    /// <summary>Represents a course offered by the institution.</summary>
    public class Course
    {
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;   // e.g. "CS101"
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // ── Navigation: one Course → many Grades, Attendances ────────────────
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
