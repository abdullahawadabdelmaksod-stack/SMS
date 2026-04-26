using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    /// <summary>Core student entity.</summary>
    public class Student : AuditableEntity
    {
        public int    StudentId { get; set; }
        public string Name       { get; set; } = string.Empty;

        public int Age
        {
            get
            {
                int age = DateTime.Today.Year - BirthDate.Year;
                if (BirthDate > DateOnly.FromDateTime(DateTime.Today.AddYears(-age))) age--;
                return age;
            }
            set { } // Keep setter so EF Core can map the column, but we always calculate it
        }

        public string Department { get; set; } = string.Empty;
        public string Phone      { get; set; } = string.Empty;
        public DateOnly BirthDate{ get; set; }
        public string Level      { get; set; } = string.Empty;

        // ── Navigation: one Student → many Grades, Attendances ───────────────
        public ICollection<Grade>      Grades      { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
