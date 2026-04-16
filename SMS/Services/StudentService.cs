using System.Collections.Generic;
using SMS.Models;
using SMS.Repository;

namespace SMS.Services
{
    public class StudentService
    {
        private readonly StudentRepository _repo = new StudentRepository();

        public List<Student> GetStudents()
        {
            return _repo.GetAll();
        }

        public void AddStudent(Student s)
        {
            if (string.IsNullOrWhiteSpace(s.Name))
                throw new System.Exception("Name is required");

            _repo.Add(s);
        }

        public void UpdateStudent(Student s)
        {
            _repo.Update(s);
        }

        public void DeleteStudent(int id)
        {
            _repo.Delete(id);
        }

        public List<Student> Search(string name)
        {
            return _repo.Search(name);
        }
    }
}
