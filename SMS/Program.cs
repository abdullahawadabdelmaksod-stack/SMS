using Microsoft.EntityFrameworkCore;
using SMS.Data;

namespace SMS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // ── Show login immediately, seed DB in the background ─────────────
            // EnsureCreated + SeedAsync are I/O-bound; running them on the UI thread
            // caused the 3-5 second black-window delay on first launch.
            Task.Run(async () =>
            {
                try 
                {
                    using var db = new AppDbContext();
                    db.Database.EnsureCreated();
                    
                    // Safely update schema for the new Professor table without dropping the DB
                    string sql = @"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Professors')
                        BEGIN
                            CREATE TABLE Professors (
                                ProfessorId INT IDENTITY(1,1) PRIMARY KEY,
                                Name NVARCHAR(150) NOT NULL,
                                Email NVARCHAR(100) NULL,
                                Department NVARCHAR(100) NULL,
                                Phone NVARCHAR(20) NULL,
                                CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                                UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                            );
                        END

                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'ProfessorId' AND Object_ID = Object_ID('Courses'))
                        BEGIN
                            ALTER TABLE Courses ADD ProfessorId INT NULL;
                            ALTER TABLE Courses ADD CONSTRAINT FK_Courses_Professors FOREIGN KEY (ProfessorId) REFERENCES Professors(ProfessorId) ON DELETE SET NULL;
                        END

                        -- Add DB constraints for User Validation
                        ALTER TABLE Users ALTER COLUMN Username NVARCHAR(50) NOT NULL;
                        ALTER TABLE Users ALTER COLUMN Password NVARCHAR(255) NOT NULL;
                        ALTER TABLE Users ALTER COLUMN Role NVARCHAR(20) NOT NULL;
                        
                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
                        BEGIN
                            CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
                        END
                    ";
                    await db.Database.ExecuteSqlRawAsync(sql);

                    await DbSeeder.SeedAsync(db);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Startup Error: {ex.Message}\n{ex.StackTrace}", "Database Initialization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            // The task runs concurrently; if a user searches before seeding
            // completes the query will simply return 0 rows — safe, no crash.

            Application.Run(new LoginForm());
        }
    }
}