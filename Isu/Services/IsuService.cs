using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuService : IIsuService
    {
        private readonly uint _groupCapacity;
        private readonly List<Student> _allStudents;
        private readonly List<Group> _allGroups;
        private int _uniqueId;

        public IsuService(int starterId = 300000, uint groupCapacity = 20)
        {
            _uniqueId = starterId;
            _groupCapacity = groupCapacity;
            _allStudents = new List<Student>();
            _allGroups = new List<Group>();
        }

        public Group AddGroup(string name)
        {
            if (!Group.CheckGroupName(name))
                throw new IsuException($"{name} is invalid group name!");
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
            return (from @group in _allGroups where @group.GroupName == groupName select @group.Students).FirstOrDefault();
        }

        public List<Student> FindStudents(CourseNumber course)
        {
            var studentsOnCourse = new List<Student>();
            foreach (Group @group in _allGroups.Where(@group => @group.GroupName[2] - '0' == course.Number))
            {
                studentsOnCourse.AddRange(@group.Students);
            }

            return studentsOnCourse;
        }

        public Group FindGroup(string groupName)
        {
            return _allGroups.FirstOrDefault(@group => @group.GroupName == groupName);
        }

        public List<Group> FindGroups(CourseNumber course)
        {
            return _allGroups.Where(@group => @group.GroupName[2] - '0' == course.Number).ToList();
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            student.ChangeGroup(newGroup);
        }
    }
}