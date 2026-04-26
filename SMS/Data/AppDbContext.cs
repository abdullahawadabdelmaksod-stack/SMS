using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.Data
{
    public class AppDbContext : DbContext
    {
        // ── Tables ───────────────────────────────────────────────────────────
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        // ── Connection ───────────────────────────────────────────────────────
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(
                @"Server=(LocalDB)\MSSQLLocalDB;Database=SMS_DB;Trusted_Connection=True;");

        // ── Fluent API: relationships, constraints, indexes ──────────────────
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Student ──────────────────────────────────────────────────────
            modelBuilder.Entity<Student>(e =>
            {
                e.ToTable("Students");
                e.HasKey(s => s.StudentId);
                e.Property(s => s.Name).IsRequired().HasMaxLength(150);
                e.Property(s => s.Department).IsRequired().HasMaxLength(5);
                e.Property(s => s.Age).IsRequired();
                e.Property(s => s.Phone).HasMaxLength(20);
                e.Property(s => s.Level).HasMaxLength(50);
            });

            // ── Course ───────────────────────────────────────────────────────
            modelBuilder.Entity<Course>(e =>
            {
                e.ToTable("Courses");
                e.HasKey(c => c.CourseId);
                e.Property(c => c.CourseId).ValueGeneratedNever();
                e.Property(c => c.Name).IsRequired().HasMaxLength(200);
                e.Property(c => c.Code).IsRequired().HasMaxLength(20);
                e.Property(c => c.Description).HasMaxLength(500);
                e.Property(c => c.Credits).IsRequired();
                e.HasIndex(c => c.Code).IsUnique();   // course codes must be unique
            });

            // ── Grade ────────────────────────────────────────────────────────
            // Student (1) ──< Grade (many)
            // Course  (1) ──< Grade (many)
            modelBuilder.Entity<Grade>(e =>
            {
                e.ToTable("Grades");
                e.HasKey(g => g.GradeId);
                e.Property(g => g.Score).IsRequired();
                e.Property(g => g.LetterGrade).HasMaxLength(2);
                e.Property(g => g.Semester).HasMaxLength(20);

                e.HasOne(g => g.Student)
                 .WithMany(s => s.Grades)
                 .HasForeignKey(g => g.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);   // delete student → delete grades

                e.HasOne(g => g.Course)
                 .WithMany(c => c.Grades)
                 .HasForeignKey(g => g.CourseId)
                 .OnDelete(DeleteBehavior.Restrict);  // prevent accidental course delete
            });

            // ── Attendance ───────────────────────────────────────────────────
            // Student (1) ──< Attendance (many)
            // Course  (1) ──< Attendance (many)
            modelBuilder.Entity<Attendance>(e =>
            {
                e.ToTable("Attendances");
                e.HasKey(a => a.AttendanceId);
                e.Property(a => a.Date).IsRequired();
                e.Property(a => a.IsPresent).IsRequired();
                e.Property(a => a.Notes).HasMaxLength(300);

                e.HasOne(a => a.Student)
                 .WithMany(s => s.Attendances)
                 .HasForeignKey(a => a.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);   // delete student → delete records

                e.HasOne(a => a.Course)
                 .WithMany(c => c.Attendances)
                 .HasForeignKey(a => a.CourseId)
                 .OnDelete(DeleteBehavior.Restrict);  // prevent accidental course delete

                // Composite index: one record per student per course per day
                e.HasIndex(a => new { a.StudentId, a.CourseId, a.Date }).IsUnique();
            });

            // ── Auditable Defaults ───────────────────────────────────────────
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entity.ClrType))
                {
                    modelBuilder.Entity(entity.ClrType).Property("CreatedAt").HasDefaultValueSql("GETUTCDATE()");
                    modelBuilder.Entity(entity.ClrType).Property("UpdatedAt").HasDefaultValueSql("GETUTCDATE()");
                }
            }
        }

        // ── Audit Auto-stamping ──────────────────────────────────────────────
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<AuditableEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }
    }
}
