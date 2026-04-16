using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // اتأكد إن السطر ده مكتوب صح
            options.UseSqlServer("Server=.;Database=SMS_DB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
