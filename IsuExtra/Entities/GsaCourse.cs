using System;
using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Tools;
using IsuExtra.Models;

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
            MegaFaculty = megaFaculty ?? throw new ArgumentNullException(
                nameof(megaFaculty),
                $"{nameof(megaFaculty)} can't be null!");

            Name = name ?? throw new ArgumentNullException(
                nameof(name),
                $"{nameof(name)} can't be null!");

            CourseNumber = courseNumber ?? throw new ArgumentNullException(
                nameof(courseNumber),
                $"{nameof(courseNumber)} can't be null!");

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
            TimeStamp timeStamp,
            Teacher teacher,
            Room room)
        {
            var gsaClass = new GsaClass(gsaCourse, timeStamp, teacher, room);
            _gsaClasses.Add(gsaClass);
            return gsaClass;
        }

        internal void DeleteGsaClass(GsaClass gsaClass)
        {
            if (gsaClass == null)
            {
                throw new ArgumentNullException(
                    nameof(gsaClass),
                    $"{nameof(gsaClass)} can't be null!");
            }

            if (!_gsaClasses.Remove(gsaClass))
                throw new IsuException("GsaClass not on this GsaCourse!");

            gsaClass.DeleteGsaClass();
        }

        internal void AddStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(
                    nameof(student),
                    $"{nameof(student)} can't be null!");
            }

            if (_students.Count == Capacity)
                throw new IsuException($"Limit of students {Capacity} reached!");

            _students.Add(student);
        }

        internal void DeleteStudent(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(
                    nameof(student),
                    $"{nameof(student)} can't be null");
            }

            if (!_students.Remove(student))
                throw new IsuException($"Student {student.Name} not in this GsaCourse!");
        }
    }
}