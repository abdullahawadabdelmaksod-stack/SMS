namespace SMS.Models
{
    /// <summary>Core student entity.</summary>
    public class Student
    {
        public int    StudentId { get; set; }
        public string Name       { get; set; } = string.Empty;
        public int    Age        { get; set; }
        public string Department { get; set; } = string.Empty;

        // ── Navigation: one Student → many Grades, Attendances ───────────────
        public ICollection<Grade>      Grades      { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
