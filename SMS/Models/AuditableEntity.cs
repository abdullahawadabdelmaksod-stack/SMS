namespace SMS.Models
{
    /// <summary>
    /// Shared audit timestamps — inherited by every entity.
    /// <c>SaveChanges</c> in AppDbContext auto-stamps these.
    /// </summary>
    public abstract class AuditableEntity
    {
        /// <summary>UTC timestamp when the record was first inserted.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp of the most recent update.</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
