using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private const int DefaultStarterId = 300000;
        private const uint DefaultGroupCapacity = 20;
        private readonly uint _groupCapacity;
        private readonly List<Student> _allStudents;
        private readonly List<Group> _allGroups;
        private int _uniqueId;

        public IsuService(int starterId = DefaultStarterId, uint groupCapacity = DefaultGroupCapacity)
        {
            _uniqueId = starterId;
            _groupCapacity = groupCapacity;
            _allStudents = new List<Student>();
            _allGroups = new List<Group>();
        }

        public Group AddGroup(string name)
        {
            var newGroup = new Group(name, _groupCapacity);
            _allGroups.Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            var newStudent = new Student(name, _uniqueId++);
            newStudent.ChangeGroup(group);
            _allStudents.Add(newStudent);
            return newStudent;
        }

        public Student GetStudent(int id)
        {
            foreach (Student student in _allStudents.Where(student => student.Id == id))
            {
                return student;
            }

            throw new IsuException($"Student with id {id} doesn't exist");
        }

        public Student FindStudent(string name)
        {
            return _allStudents.FirstOrDefault(student => student.Name == name);
        }

        public List<Student> FindStudents(string groupName)
        {
            return _allGroups.Where(group => group.GroupName == groupName)
                             .Select(group => group.Students)
                             .FirstOrDefault();
        }

        public List<Student> FindStudents(CourseNumber course)
        {
            return _allGroups.Where(group => group.Course == course.Number)
                             .SelectMany(group => group.Students)
                             .ToList();
        }

        public Group FindGroup(string groupName)
        {
            return _allGroups.FirstOrDefault(group => group.GroupName == groupName);
        }

        public List<Group> FindGroups(CourseNumber course)
        {
            return _allGroups.Where(group => group.Course == course.Number).ToList();
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            student.ChangeGroup(newGroup);
        }
    }
}