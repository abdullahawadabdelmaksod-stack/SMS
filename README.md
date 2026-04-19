Technical Architecture Overview
​The system follows a Layered Architecture (N-Tier), which promotes the Separation of Concerns (SoC) principle. This ensures that the user interface is decoupled from the data access logic, making the system maintainable and scalable.

​1. Presentation Layer (UI)
​Technology: Windows Forms (WinForms) with MaterialSkin.2.
​Design Pattern: Event-Driven Architecture.
​Role: Handles user interactions, input validation, and rendering the "Dark Mode" aesthetic. It communicates with the Service layer to fetch or send data without knowing how the database works.

​2. Logic & Service Layer (BLL)
​Technology: C# Methods & Business Rules.
​Role: Acts as a bridge. It contains the logic for student enrollment rules, data formatting (e.g., ensuring the Age is a positive integer), and coordinating between the UI and the Repository.

​3. Data Access Layer (DAL / Repository)
​Technology: ADO.NET / SQL Client.
​Pattern: Repository Pattern.
​Role: This layer abstracts the SQL queries. Instead of writing SELECT * FROM Students inside a button click, you call a method like _studentRepository.GetAll(). This allows you to swap SQL Server for another database (like MySQL) in the future without touching the UI code.

​4. Persistence Layer (Database)
​Technology: SQL Server.
​Role: The "Source of Truth." It stores data in relational tables with primary keys (ID) to ensure Data Integrity.

​🔹 Dependency Management
​Instead of hard-coding values, the system uses a Connection String management approach. This follows the DRY (Don't Repeat Yourself) principle, allowing the entire application to point to a new server by changing just one line in a configuration file.

​🔹 UI/UX Optimization
​The implementation of a Dark Theme using Material Design principles isn't just for looks; it's about reducing visual fatigue and following modern Human Interface Guidelines (HIG). The use of MaterialCards provides visual elevation, helping the user distinguish between "Input Areas" and "Display Areas."

​🔹 CRUD Lifecycle
​The application implements the full CRUD Lifecycle:
​Create: Sanitized input sent to the database.
​Read: Optimized fetching into a DataGridView.
​Update: Delta-changes applied to existing records.
​Delete: Permanent record removal from the persistence layer.
