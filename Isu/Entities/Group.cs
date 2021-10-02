using System;
using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Models;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private readonly uint _groupCapacity;
        private readonly List<GroupStudyClass> _timeTable;
        private readonly List<Student> _students;
        internal Group(GroupName groupName, uint groupCapacity)
        {
            GroupName = groupName ?? throw new ArgumentNullException(
                nameof(groupName),
                $"{nameof(groupName)} can't be null!");

            _students = new List<Student>();
            _groupCapacity = groupCapacity;
            _timeTable = new List<GroupStudyClass>();
        }

        public GroupName GroupName { get; }
        public CourseNumber CourseNumber => GroupName.Course.CourseNumber;
        public IReadOnlyCollection<Student> Students => _students;
        public IReadOnlyCollection<GroupStudyClass> TimeTable => _timeTable;

        internal void AddStudentToGroup(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(
                    nameof(student),
                    $"{nameof(student)} can't be null!");
            }

            if (Students.Count == _groupCapacity)
                throw new IsuException("Group reached limit of students!");
            if (student.Group != null)
                throw new IsuException("Student is already has group!");
            _students.Add(student);
        }

        internal void DeleteStudentFromGroup(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(
                    nameof(student),
                    $"{nameof(student)} can't be null!");
            }

            if (!_students.Remove(student))
                throw new IsuException("Student wasn't in this group!");
        }

        internal void AddStudyClassToTimeTable(GroupStudyClass groupStudyClass)
        {
            if (groupStudyClass == null)
            {
                throw new ArgumentNullException(
                    nameof(groupStudyClass),
                    $"{nameof(groupStudyClass)} can't be null!");
            }

            if (_timeTable.Any(existingClass => groupStudyClass.TimeStamp.CheckIfIntersects(existingClass.TimeStamp)))
                throw new IsuException($"StudyClass intersects with group {GroupName} TimeTable!");

            _timeTable.Add(groupStudyClass);
        }

        internal void RemoveStudyClassFromTimeTable(GroupStudyClass groupStudyClass)
        {
            if (groupStudyClass == null)
                throw new ArgumentNullException(nameof(groupStudyClass), $"{nameof(groupStudyClass)} can't be null!");

            _timeTable.Remove(groupStudyClass);
        }
    }
}
