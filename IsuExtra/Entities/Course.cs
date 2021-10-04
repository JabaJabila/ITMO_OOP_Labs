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
            CourseNumber = courseNumber ?? throw new ArgumentNullException(nameof(courseNumber));

            Faculty = faculty ?? throw new ArgumentNullException(nameof(faculty));

            _subjects = new List<Subject>();
            _groups = new List<Group>();
        }

        public CourseNumber CourseNumber { get; }
        public Faculty Faculty { get; }
        public IReadOnlyCollection<Subject> Subjects => _subjects;
        public IReadOnlyCollection<Group> Groups => _groups;

        public void AddGroupToCourse(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (_groups.Contains(group))
                throw new IsuException($"Group {group.GroupName.Name} is already on this course!");

            if (group.GroupName.FacultyLetter != Faculty.Letter ||
                !group.CourseNumber.Equals(CourseNumber))
                throw new IsuException($"Group {group.GroupName.Name} can't be on this course!");

            _groups.Add(group);
        }

        public void DeleteGroupFromCourse(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (!_groups.Remove(group))
                throw new IsuException($"Group {group.GroupName.Name} is not on this course!");
        }

        public Subject AddSubjectToCourse(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var subject = new Subject(name);
            _subjects.Add(subject);
            return subject;
        }

        public void DeleteSubjectFromCourse(Subject subject)
        {
            if (subject == null)
                throw new ArgumentNullException(nameof(subject));

            if (!_subjects.Remove(subject))
            {
                throw new IsuException($"StudyClass {subject.Name} not exists " +
                                       $"in {Faculty.Name} on {CourseNumber} course!");
            }

            subject.Delete();
        }
    }
}