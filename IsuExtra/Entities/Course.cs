using System;
using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Course
    {
        private readonly List<Subject> _subjects;
        private readonly List<Group> _groups;

        internal Course(CourseNumber courseNumber, Faculty faculty)
        {
            CourseNumber = courseNumber ?? throw new ArgumentNullException(
                nameof(courseNumber),
                $"{nameof(courseNumber)} can't be null!");

            Faculty = faculty ?? throw new ArgumentNullException(
                nameof(faculty),
                $"{nameof(faculty)} can't be null!");

            _subjects = new List<Subject>();
            _groups = new List<Group>();
        }

        public CourseNumber CourseNumber { get; }
        public Faculty Faculty { get; }
        public IReadOnlyCollection<Subject> Subjects => _subjects;
        public IReadOnlyCollection<Group> Groups => _groups;

        internal void AddGroupOnCourse(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(
                    nameof(group),
                    $"{nameof(group)} can't be null!");
            }

            if (_groups.Contains(group))
                throw new IsuException($"Group {group.GroupName.Name} is already on this course!");

            _groups.Add(group);
        }

        internal void DeleteGroupFromCourse(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(
                    nameof(group),
                    $"{nameof(group)} can't be null!");
            }

            if (!_groups.Remove(group))
                throw new IsuException($"Group {group.GroupName.Name} is not on this course!");
        }

        internal Subject AddSubject(string name)
        {
            var subject = new Subject(name);
            _subjects.Add(subject);
            return subject;
        }

        internal void DeleteSubject(Subject subject)
        {
            if (!_subjects.Remove(subject))
            {
                throw new IsuException($"StudyClass {subject.Name} not exists " +
                                       $"in {Faculty.Name} on {CourseNumber} course!");
            }

            subject.Delete();
        }
    }
}