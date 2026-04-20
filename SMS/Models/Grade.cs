namespace SMS.Models
{
    /// <summary>
    /// A grade record linking one Student to one Course.
    /// Student → Grade is one-to-many; Course → Grade is one-to-many.
    /// </summary>
    public class Grade
    {
        public int    GradeId   { get; set; }
        public double Score     { get; set; }          // 0.0 – 100.0
        public string LetterGrade { get; set; } = string.Empty;  // A, B, C, D, F
        public string Semester  { get; set; } = string.Empty;    // e.g. "2024-S1"

        // ── FK → Student ─────────────────────────────────────────────────────
        public int     StudentId { get; set; }
        public Student Student   { get; set; } = null!;

        // ── FK → Course ──────────────────────────────────────────────────────
        public int    CourseId { get; set; }
        public Course Course   { get; set; } = null!;
    }
}
