using System;
using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class GsaCourse
    {
        private readonly List<GsaClass> _gsaClasses;
        private readonly List<Student> _students;

        internal GsaCourse(
            MegaFaculty megaFaculty,
            string name,
            CourseNumber courseNumber,
            uint capacity)
        {
            MegaFaculty = megaFaculty ?? throw new ArgumentNullException(nameof(megaFaculty));

            Name = name ?? throw new ArgumentNullException(nameof(name));

            CourseNumber = courseNumber ?? throw new ArgumentNullException(nameof(courseNumber));

            Capacity = capacity;
            _gsaClasses = new List<GsaClass>();
            _students = new List<Student>();
        }

        public MegaFaculty MegaFaculty { get; }
        public CourseNumber CourseNumber { get; }
        public uint Capacity { get; }
        public string Name { get; }
        public IReadOnlyCollection<GsaClass> GsaClasses => _gsaClasses;
        public IReadOnlyCollection<Student> Students => _students;

        internal GsaClass AddGsaClass(
            GsaCourse gsaCourse,
            StudyStreamPeriod studyStreamPeriod,
            Teacher teacher,
            Room room)
        {
            var gsaClass = new GsaClass(gsaCourse, studyStreamPeriod, teacher, room);
            _gsaClasses.Add(gsaClass);
            return gsaClass;
        }

        internal void DeleteGsaClass(GsaClass gsaClass)
        {
            if (gsaClass == null)
                throw new ArgumentNullException(nameof(gsaClass));

            if (!_gsaClasses.Remove(gsaClass))
                throw new IsuException("GsaClass not on this GsaCourse!");

            gsaClass.Delete();
        }

        internal void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (_students.Count == Capacity)
                throw new IsuException($"Limit of students {Capacity} reached!");

            _students.Add(student);
        }

        internal void DeleteStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (!_students.Remove(student))
                throw new IsuException($"Student {student.Name} not in this GsaCourse!");
        }
    }
}