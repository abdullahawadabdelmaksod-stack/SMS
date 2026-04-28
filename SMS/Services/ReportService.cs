using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMS.Models;
using System;
using System.IO;
using System.Linq;
using SMS.Services;

namespace SMS.Services
{
    public class ReportService
    {
        private readonly GradeService _gradeService;
        private readonly AttendanceService _attendanceService;

        public ReportService()
        {
            _gradeService = new GradeService();
            _attendanceService = new AttendanceService();
        }

        public void GenerateStudentReportPdf(Student student, string outputPath)
        {
            // Set the license type for QuestPDF to Community
            QuestPDF.Settings.License = LicenseType.Community;

            var gpa = _gradeService.GetGPA(student.StudentId);
            var attendanceRate = _attendanceService.GetAttendanceRate(student.StudentId);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(x => ComposeContent(x, student, gpa, attendanceRate));
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf(outputPath);
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Student Management System").FontSize(20).SemiBold().FontColor(Colors.Indigo.Darken2);
                    column.Item().Text("Official Student Report").FontSize(14).FontColor(Colors.Grey.Darken2);
                    column.Item().PaddingTop(5).Text($"Generated: {DateTime.Now:MMMM dd, yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                });

                // Optional: Logo space
                row.ConstantItem(100).AlignRight().Text("SMS").FontSize(24).Bold().FontColor(Colors.Teal.Medium);
            });
        }

        private void ComposeContent(IContainer container, Student student, double gpa, double attendanceRate)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                // 1. Student Information
                column.Item().Text("Student Information  |  معلومات الطالب").FontSize(14).SemiBold().FontColor(Colors.Indigo.Medium);
                column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Name: {student.Name}");
                        col.Item().Text($"Student ID: {student.StudentId}");
                    });
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Department: {student.Department}");
                        col.Item().Text($"Level: {student.Level}");
                    });
                });

                column.Item().PaddingTop(15).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Current GPA: {gpa:F2} / 4.0").SemiBold();
                    });
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text($"Attendance Rate: {attendanceRate:F1}%").SemiBold();
                    });
                });

                column.Item().PaddingTop(25);

                // 2. Grades Table
                column.Item().Text("Course Grades  |  درجات المقررات").FontSize(14).SemiBold().FontColor(Colors.Indigo.Medium);
                column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1); // Code
                        columns.RelativeColumn(3); // Name
                        columns.RelativeColumn(1); // Semester
                        columns.RelativeColumn(1); // Score
                        columns.RelativeColumn(1); // Grade
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Code");
                        header.Cell().Element(CellStyle).Text("Course Name");
                        header.Cell().Element(CellStyle).Text("Semester");
                        header.Cell().Element(CellStyle).AlignRight().Text("Score");
                        header.Cell().Element(CellStyle).AlignCenter().Text("Grade");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var grade in student.Grades)
                    {
                        table.Cell().Element(CellStyle).Text(grade.Course.Code);
                        table.Cell().Element(CellStyle).Text(grade.Course.Name);
                        table.Cell().Element(CellStyle).Text(grade.Semester);
                        table.Cell().Element(CellStyle).AlignRight().Text(grade.Score.ToString());
                        table.Cell().Element(CellStyle).AlignCenter().Text(grade.LetterGrade).SemiBold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });

                column.Item().PaddingTop(25);

                // 3. Attendance Table
                column.Item().Text("Attendance Records  |  سجلات الحضور").FontSize(14).SemiBold().FontColor(Colors.Indigo.Medium);
                column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Date
                        columns.RelativeColumn(3); // Course
                        columns.RelativeColumn(2); // Status
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Date");
                        header.Cell().Element(CellStyle).Text("Course Name");
                        header.Cell().Element(CellStyle).Text("Status");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var att in student.Attendances.OrderByDescending(a => a.Date))
                    {
                        table.Cell().Element(CellStyle).Text(att.Date.ToShortDateString());
                        table.Cell().Element(CellStyle).Text(att.Course.Name);
                        table.Cell().Element(CellStyle).Text(att.IsPresent ? "Present" : "Absent")
                             .FontColor(att.IsPresent ? Colors.Green.Darken2 : Colors.Red.Darken2);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
                x.Span(" of ");
                x.TotalPages();
            });
        }
    }
}
