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
        public DbSet<Professor> Professors { get; set; }
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
                e.Property(s => s.Department).IsRequired().HasMaxLength(100); // fix: was 5
                e.Ignore(s => s.Age);   // calculated from BirthDate — not stored
                e.Property(s => s.Phone).HasMaxLength(20);
                e.Property(s => s.Level).HasMaxLength(50);
            });

            // ── User ─────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(u => u.UserId);
                e.Property(u => u.Username).IsRequired().HasMaxLength(50);
                e.HasIndex(u => u.Username).IsUnique(); // Ensure usernames are unique!
                e.Property(u => u.Password).IsRequired().HasMaxLength(255);
                e.Property(u => u.Role).IsRequired().HasMaxLength(20);
            });

            // ── Course ───────────────────────────────────────────────────────
            modelBuilder.Entity<Course>(e =>
            {
                e.ToTable("Courses");
                e.HasKey(c => c.CourseId);
                // ValueGeneratedOnAdd (default) — seeder uses auto IDs
                e.Property(c => c.Name).IsRequired().HasMaxLength(200);
                e.Property(c => c.Code).IsRequired().HasMaxLength(20);
                e.Property(c => c.Description).HasMaxLength(500);
                e.Property(c => c.Credits).IsRequired();
                e.HasIndex(c => c.Code).IsUnique();

                e.HasOne(c => c.Professor)
                 .WithMany(p => p.Courses)
                 .HasForeignKey(c => c.ProfessorId)
                 .OnDelete(DeleteBehavior.SetNull); // If a professor is deleted, keep the course but set ProfessorId to NULL
            });

            // ── Professor ────────────────────────────────────────────────────
            modelBuilder.Entity<Professor>(e =>
            {
                e.ToTable("Professors");
                e.HasKey(p => p.ProfessorId);
                e.Property(p => p.Name).IsRequired().HasMaxLength(150);
                e.Property(p => p.Email).HasMaxLength(100);
                e.Property(p => p.Phone).HasMaxLength(20);
                e.Property(p => p.Department).HasMaxLength(100);
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

                // No unique index — multiple records per session day are allowed
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
