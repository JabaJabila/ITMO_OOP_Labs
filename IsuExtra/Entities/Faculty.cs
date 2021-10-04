using System;
using System.Collections.Generic;
using Isu.DataTypes;

namespace IsuExtra.Entities
{
    public class Faculty
    {
        private readonly List<Course> _courses;

        internal Faculty(string name, char letterOfFaculty, MegaFaculty megaFaculty)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            Letter = letterOfFaculty;
            MegaFaculty = megaFaculty ?? throw new ArgumentNullException(nameof(megaFaculty));

            _courses = new List<Course>();
        }

        public string Name { get; }
        public char Letter { get; }
        public IReadOnlyCollection<Course> Courses => _courses;
        public MegaFaculty MegaFaculty { get; }

        internal Course AddCourse(CourseNumber number)
        {
            var course = new Course(number, this);
            _courses.Add(course);
            return course;
        }
    }
}