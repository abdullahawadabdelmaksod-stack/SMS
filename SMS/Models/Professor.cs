namespace SMS.Models
{
    public class Professor : AuditableEntity
    {
        public int ProfessorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }

        // ── Navigation ───────────────────────────────────────────────────────
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
