using System;
using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Tools;

namespace Isu.Entities
{
    public class Course
    {
        private readonly List<Subject> _subjects;

        internal Course(CourseNumber courseNumber, Faculty faculty)
        {
            CourseNumber = courseNumber ?? throw new ArgumentNullException(
                nameof(courseNumber),
                $"{nameof(courseNumber)} can't be null!");

            Faculty = faculty ?? throw new ArgumentNullException(
                nameof(faculty),
                $"{nameof(faculty)} can't be null!");

            _subjects = new List<Subject>();
        }

        public CourseNumber CourseNumber { get; }
        public Faculty Faculty { get; }
        public IReadOnlyCollection<Subject> Subjects => _subjects;

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