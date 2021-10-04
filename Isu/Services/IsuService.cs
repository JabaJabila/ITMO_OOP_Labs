using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private const int DefaultStarterId = 100000;
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

        public Group AddGroup(GroupName name)
        {
            if (_allGroups.Any(group => group.GroupName.Equals(name)))
                throw new IsuException($"Group with name {name.Name} already exists!");

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
            Student student = _allStudents.FirstOrDefault(student => student.Id == id);
            return student ?? throw new IsuException($"Student with id {id} doesn't exist");
        }

        public Student FindStudent(string name)
        {
            return _allStudents.FirstOrDefault(student => student.Name == name);
        }

        public IReadOnlyList<Student> FindStudents(GroupName groupName)
        {
            return _allGroups.Where(group => group.GroupName.Equals(groupName))
                             .SelectMany(group => group.Students)
                             .ToList();
        }

        public IReadOnlyList<Student> FindStudents(CourseNumber course)
        {
            return _allGroups.Where(group => group.CourseNumber.Equals(course))
                             .SelectMany(group => group.Students)
                             .ToList();
        }

        public Group FindGroup(GroupName groupName)
        {
            return _allGroups.FirstOrDefault(group => group.GroupName.Equals(groupName));
        }

        public IReadOnlyList<Group> FindGroups(CourseNumber course)
        {
            return _allGroups.Where(group => group.CourseNumber.Equals(course)).ToList();
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            student.ChangeGroup(newGroup);
        }
    }
}