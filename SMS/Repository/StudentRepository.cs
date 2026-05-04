using System.Collections.Generic;
using System.Linq;
using SMS.Data;
using SMS.Models;

namespace SMS.Repository
{
    public class StudentRepository
    {
        private readonly AppDbContext _context = new AppDbContext();

        public List<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            var existing = _context.Students.Find(student.StudentId);

            if (existing != null)
            {
                existing.Name = student.Name;
                existing.Department = student.Department;
                existing.Phone = student.Phone;
                existing.BirthDate = student.BirthDate;
                existing.Level = student.Level;

                _context.SaveChanges();
            }

        }

        public void Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        public List<Student> Search(string name)
        {
            return _context.Students
                .Where(s => s.Name.Contains(name))
                .ToList();
        }

        public bool NameExists(string name, int excludeId = 0)
        {
            return _context.Students.Any(s => s.Name.ToLower() == name.ToLower() && s.StudentId != excludeId);
        }
    }
}