using Microsoft.EntityFrameworkCore;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SMS.Data
{
    public class AppDbContext : DbContext
    {
     public DbSet<Student> Students { get; set; }
      public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=SMS_DB;Trusted_Connection=True;"); 
        }
       
    }
}
