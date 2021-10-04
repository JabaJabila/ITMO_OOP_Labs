using System;
using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private readonly uint _groupCapacity;
        private readonly List<Student> _students;
        internal Group(GroupName groupName, uint groupCapacity)
        {
            GroupName = groupName ?? throw new ArgumentNullException(nameof(groupName));

            _students = new List<Student>();
            _groupCapacity = groupCapacity;
        }

        public GroupName GroupName { get; }
        public CourseNumber CourseNumber => GroupName.CourseNumber;
        public IReadOnlyCollection<Student> Students => _students;

        internal void AddStudentToGroup(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (Students.Count == _groupCapacity)
                throw new IsuException("Group reached limit of students!");

            if (student.Group != null)
                throw new IsuException("Student is already has group!");

            _students.Add(student);
        }

        internal void DeleteStudentFromGroup(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (!_students.Remove(student))
                throw new IsuException($"Student {student.Name} is not in this group!");
        }
    }
}
