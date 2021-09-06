using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private readonly Course[] _courses;
        private readonly uint _groupCapacity;
        private readonly Dictionary<int, Student> _allStudents;
        private readonly Dictionary<string, Group> _allGroups;
        private int _uniqueId;

        public IsuService(int starterId = 300000, uint groupCapacity = 20)
        {
            _uniqueId = starterId;
            _courses = new Course[4];
            _groupCapacity = groupCapacity;
            _allStudents = new Dictionary<int, Student>();
            _allGroups = new Dictionary<string, Group>();
            for (int i = 0; i < 4; ++i)
            {
                _courses[i] = new Course(i + 1);
            }
        }

        public Group AddGroup(string name)
        {
            if (!Group.CheckGroupName(name))
                throw new IsuException("Invalid group name!");
            Course course = _courses[name[2] - '0' - 1];
            var newGroup = new Group(name, _groupCapacity);
            course.AddGroupToCourse(newGroup);
            _allGroups[name] = newGroup;
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            var newStudent = new Student(name, _uniqueId);
            _uniqueId++;
            newStudent.ChangeGroup(group);
            _allStudents[newStudent.Id] = newStudent;
            return newStudent;
        }

        public Student GetStudent(int id)
        {
            if (_allStudents.ContainsKey(id))
                return _allStudents[id];

            throw new IsuException($"Student with id {id} doesn't exist");
        }

        public Student FindStudent(string name)
        {
            foreach (KeyValuePair<int, Student> item in _allStudents)
            {
                if (item.Value.Name == name)
                    return item.Value;
            }

            return null;
        }

        public List<Student> FindStudents(string groupName)
        {
            if (_allGroups.ContainsKey(groupName))
                return _allGroups[groupName].Students;

            return null;
        }

        public List<Student> FindStudents(CourseNumber course)
        {
            throw new System.NotImplementedException();
        }

        public Group FindGroup(string groupName)
        {
            if (_allGroups.ContainsKey(groupName))
                return _allGroups[groupName];

            return null;
        }

        public List<Group> FindGroups(CourseNumber course)
        {
            return _courses[course.Number - 1].Groups;
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            student.ChangeGroup(newGroup);
        }
    }
}