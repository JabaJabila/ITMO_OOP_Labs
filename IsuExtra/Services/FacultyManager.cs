using System;
using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Tools;
using IsuExtra.Entities;

namespace IsuExtra.Services
{
    public class FacultyManager
    {
        private const int DefaultFirstMegaFacultyId = 1;
        private readonly List<MegaFaculty> _megaFaculties;
        private readonly List<char> _facultyLetters;
        private int _uniqueId;

        public FacultyManager(int firstUniqueId = DefaultFirstMegaFacultyId)
        {
            _megaFaculties = new List<MegaFaculty>();
            _facultyLetters = new List<char>();
            _uniqueId = firstUniqueId;
        }

        public IReadOnlyCollection<MegaFaculty> MegaFaculties => _megaFaculties;

        public MegaFaculty CreateMegaFaculty(string name)
        {
            var megaFaculty = new MegaFaculty(name, _uniqueId++);
            _megaFaculties.Add(megaFaculty);
            return megaFaculty;
        }

        public Faculty CreateFaculty(string name, MegaFaculty megaFaculty, char letter)
        {
            if (megaFaculty == null)
                throw new ArgumentNullException(nameof(megaFaculty));

            if (letter is < 'A' or > 'Z')
                throw new IsuException("Invalid letter of Faculty. Letter must be in range A..Z");

            if (_facultyLetters.Contains(letter))
                throw new IsuException($"Faculty with letter {letter} already exists!");

            _facultyLetters.Add(letter);
            var faculty = new Faculty(name, letter, megaFaculty);
            megaFaculty.AddFacultyToMegaFaculty(faculty);
            return faculty;
        }

        public Course AddCourseOnFaculty(Faculty faculty, CourseNumber courseNumber)
        {
            if (faculty == null)
                throw new ArgumentNullException(nameof(faculty));

            if (courseNumber == null)
                throw new ArgumentNullException(nameof(courseNumber));

            return faculty.AddCourse(courseNumber);
        }
    }
}