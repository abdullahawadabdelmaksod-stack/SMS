# Student Management System (SMS)

A full-featured desktop application for managing student academic records, built with **C# / .NET 9 WinForms** and backed by **SQL Server (LocalDB)**. The project follows a clean, layered architecture and applies professional software engineering practices throughout.

---

## Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | WinForms (.NET 9) + MaterialSkin.2 |
| ORM / Data Access | Entity Framework Core 9 (Code-First) |
| Database | SQL Server (LocalDB) |
| Language | C# 12 |
| Pattern | Repository + Service layer |

---

## Architecture

The application is structured into distinct, loosely-coupled layers:

```
SMS/
├── Models/          # Domain entities (Student, Course, Grade, Attendance, User)
├── Data/            # AppDbContext (EF Core), DbSeeder
├── Repository/      # Generic data access abstraction
├── Services/        # Business logic layer
├── Forms/           # WinForms UI (Dashboard, SMS main form, Login)
└── Resources/       # App icon, logo assets
```

- **Code-First Migrations**: Schema is fully managed through EF Core migrations. All model changes propagate to the database via `dotnet ef migrations add` / `database update`.
- **Fluent API**: All entity constraints, indexes, max-lengths, and foreign key behaviour are configured explicitly in `OnModelCreating` — no data annotation pollution on models.
- **Audit Base Class**: Every entity inherits from `AuditableEntity`, which provides `CreatedAt` and `UpdatedAt` timestamps. These are auto-stamped in a `SaveChanges` / `SaveChangesAsync` override — no manual tracking required anywhere in the codebase.
- **Seeder**: An idempotent `DbSeeder` class seeds 100 realistic records (Egyptian student names, 10 courses, 300+ grades, 900+ attendance records) on first launch. It is a no-op if data already exists.

---

## Features

### Student Management
- Full CRUD with extended profile: **Name, Age, Department, Level, Phone, Address, Birth Date**
- Cross-linked navigation — selecting a student pre-loads that student's grades and attendance in the other tabs
- Search across Name, Department, Level, and Phone

### Course Management
- Full CRUD for academic courses (Code, Credits, Description)
- Unique course code enforcement at the database level

### Grade Management
- Record scores (0–100), letter grades (A–F), and semester
- Filter grades by Student ID
- Auto-calculates aggregate average score on the dashboard

### Attendance
- Track presence/absence per student per course per day
- Unique composite index `(StudentId, CourseId, Date)` prevents duplicate entries at the DB level
- **Dual-filter search**: filter simultaneously by Student ID and/or Course ID
- 85% realistic attendance rate generated in seed data

### Dashboard
- Live stat cards: Total Students, Total Courses, Grades Recorded, Attendance Rate, Average Score
- Recent students grid auto-refreshes on navigation
- Real-time clock display

### Authentication
- Role-based `Users` table (Admin / Staff / Faculty)
- Login form with DB-backed credential validation
- Short-lived `DbContext` per request — no shared connection state

---

## Key Engineering Decisions

**1. Cascade Delete via Fluent API**
Deleting a student cascades to all their `Grade` and `Attendance` records automatically. Deleting a course is restricted — it cannot be removed while dependent records exist, preventing orphaned data.

**2. Audit Timestamps via SaveChanges Override**
Rather than manually setting `CreatedAt`/`UpdatedAt` in every handler, the `DbContext.SaveChanges` override iterates `ChangeTracker` entries and stamps timestamps only for `Added` or `Modified` entities. This is a single, central enforcement point.

**3. Projection-Only DataGridViews**
All data grids use explicit `Select(s => new { ... })` anonymous-type projections. Navigation properties (e.g. `Student.Grades`) are never bound directly to the UI, preventing lazy-load exceptions and accidental N+1 query patterns.

**4. Dynamic Layout Engine**
The right-panel editor column uses a top-down Y-coordinate layout engine (`LayoutEditorColumn`) that recalculates all control positions from the bottom of the previous element. This means adding or resizing any field reflows the entire panel automatically, with no hard-coded pixel offsets.

**5. MaterialSkin Font Override Guard**
MaterialSkin's `AddFormToManage` resets fonts when applying its theme. Fonts are re-applied in a dedicated `ApplyFonts()` method called both immediately after `skin.Theme = DARK` and again in the `Shown` event — guaranteeing the intended typography survives any framework-internal resets.

---

## Database Schema

```
Students ──< Grades      >── Courses
Students ──< Attendances >── Courses
Users
```

All tables include `CreatedAt` and `UpdatedAt` columns with `GETUTCDATE()` SQL defaults.

---

## Getting Started

```bash
# 1. Restore dependencies
dotnet restore

# 2. Apply migrations (creates LocalDB database)
dotnet ef database update --project SMS/SMS.csproj

# 3. Run
dotnet run --project SMS/SMS.csproj
```

**Default credentials:** `admin` / `admin123`

The seeder runs automatically on first launch and populates the database with 100 students and all related records.

