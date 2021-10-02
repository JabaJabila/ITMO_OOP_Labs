using System;
using System.Collections.Generic;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Models;
using Isu.Tools;
using IsuExtra.Entities;

namespace IsuExtra.Services
{
    public class GsaService
    {
        private const uint StandardGsaStreamCapacity = 30;
        private const uint CountOfGsaCoursesForStudents = 2;
        private readonly List<GsaCourse> _gsaCourses;

        public GsaService()
        {
            _gsaCourses = new List<GsaCourse>();
        }

        public IReadOnlyCollection<GsaCourse> GsaCourses => _gsaCourses;

        public GsaCourse AddGsaCourse(
            MegaFaculty megaFaculty,
            string name,
            CourseNumber courseNumber,
            uint capacity = StandardGsaStreamCapacity)
        {
            var gsaCourse = new GsaCourse(
                megaFaculty,
                name,
                courseNumber,
                capacity);

            _gsaCourses.Add(gsaCourse);
            return gsaCourse;
        }

        public GsaClass AddGsaClass(
            GsaCourse gsaCourse,
            TimeStamp timeStamp,
            Teacher teacher,
            Room room)
        {
            if (teacher.TimeTable.Any(studyStream => studyStream.TimeStamp.CheckIfIntersects(timeStamp)))
            {
                throw new IsuException($"Teacher {teacher.Name} has classes " +
                                       $"that intersects with this timestamp!");
            }

            if (room.TimeTable.Any(studyStream => studyStream.TimeStamp.CheckIfIntersects(timeStamp)))
            {
                throw new IsuException($"Room {room.Number} is used for classes " +
                                       $"that intersects with this timestamp!");
            }

            return gsaCourse.AddGsaClass(gsaCourse, timeStamp, teacher, room);
        }

        public void DeleteGsaClass(GsaClass gsaClass)
        {
            if (gsaClass == null)
            {
                throw new ArgumentNullException(
                    nameof(gsaClass),
                    $"{nameof(gsaClass)} can't be null!");
            }

            gsaClass.GsaCourse.DeleteGsaClass(gsaClass);
        }

        public void AddStudentToGsaCourse(GsaCourse gsaCourse, Student student)
        {
            if (gsaCourse == null)
            {
                throw new ArgumentNullException(
                    nameof(gsaCourse),
                    $"{nameof(gsaCourse)} can't be null!");
            }

            if (student == null)
            {
                throw new ArgumentNullException(
                    nameof(student),
                    $"{nameof(student)} can't be null!");
            }

            if (student.Group.GroupName.Faculty.MegaFaculty.Id.Equals(gsaCourse.MegaFaculty.Id))
            {
                throw new IsuException($"Student {student.Name} is already " +
                                       $"studying on MegaFaculty {gsaCourse.MegaFaculty.Name}!");
            }

            if (StudentHasEnoughGsaCourses(student))
            {
                throw new IsuException($"Student {student.Name} already has " +
                                       $"{CountOfGsaCoursesForStudents} GSA Courses!");
            }

            if (!student.Group.GroupName.Course.CourseNumber.Equals(gsaCourse.CourseNumber))
            {
                throw new IsuException($"Student's course number " +
                                       $"({student.Group.GroupName.Course.CourseNumber.Number})" +
                                       $" not equals required GSA Course's course number " +
                                       $"({gsaCourse.CourseNumber.Number})");
            }

            if (GetStudentsFromGsaCourse(gsaCourse).Contains(student))
            {
                throw new IsuException($"Student {student.Name} is already signed for this GSA course!");
            }

            if (gsaCourse.GsaClasses.Any(gsaClass => student.Group.TimeTable.Any(studyClass =>
                studyClass.TimeStamp.CheckIfIntersects(gsaClass.TimeStamp))))
            {
                throw new IsuException($"Student {student.Name} has group classes which intersects with GSA classes!");
            }

            gsaCourse.AddStudent(student);
        }

        public void DeleteStudentFromGsaCourse(GsaCourse gsaCourse, Student student)
        {
            if (gsaCourse == null)
            {
                throw new ArgumentNullException(
                    nameof(gsaCourse),
                    $"{nameof(gsaCourse)} can't be null!");
            }

            if (student == null)
            {
                throw new ArgumentNullException(
                    nameof(student),
                    $"{nameof(student)} can't be null!");
            }

            gsaCourse.DeleteStudent(student);
        }

        public IReadOnlyList<GsaCourse> GetGsaCoursesWithSameCourseNumber(CourseNumber courseNumber)
        {
            if (courseNumber == null)
            {
                throw new ArgumentNullException(
                    nameof(courseNumber),
                    $"{nameof(courseNumber)} can't be null!");
            }

            return _gsaCourses.Where(gsaCourse => gsaCourse.CourseNumber.Equals(courseNumber)).ToList();
        }

        public IReadOnlyList<Student> GetStudentsFromGsaCourse(GsaCourse gsaCourse)
        {
            if (gsaCourse == null)
            {
                throw new ArgumentNullException(
                    nameof(gsaCourse),
                    $"{nameof(gsaCourse)} can't be null!");
            }

            return gsaCourse.Students.ToList();
        }

        public IReadOnlyList<Student> GetNotSignedUpForEnoughGsaCoursesStudents(IEnumerable<Student> students)
        {
            return students.Where(student => !StudentHasEnoughGsaCourses(student)).ToList();
        }

        private bool StudentHasEnoughGsaCourses(Student student)
        {
            return _gsaCourses
                .SelectMany(gsaCourse => gsaCourse.Students)
                .Count(studentOnCourse => studentOnCourse.Id == student.Id) == CountOfGsaCoursesForStudents;
        }
    }
}