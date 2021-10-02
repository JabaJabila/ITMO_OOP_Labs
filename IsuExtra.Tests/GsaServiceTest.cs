using System;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Models;
using Isu.Services;
using Isu.Tools;
using IsuExtra.Entities;
using IsuExtra.Services;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    [TestFixture]
    public class Tests
    {
        private IsuService _isuService;
        private GsaService _gsaService;
        private FacultyManager _facultyManager;
        private CourseManager _courseManager;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService(groupCapacity: 5);
            _gsaService = new GsaService();
            _facultyManager = new FacultyManager();
            _courseManager = new CourseManager();
        }

        [TestCase("TINT", "Functional Analysis", 2)]
        public void AddMegaFacultyAndGsa_MegaFacultyHasGsa(
            string megaFacultyName,
            string gsaCourseName,
            int courseNumber)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty(megaFacultyName);
            GsaCourse course = _gsaService.AddGsaCourse(megaFaculty, gsaCourseName, new CourseNumber(courseNumber));
            Assert.AreEqual(course.MegaFaculty.Id, megaFaculty.Id);
        }

        [TestCase(
            "TINT",
            "Func Analysis",
            2,
            10,
            0,
            11,
            30,
            DayOfWeek.Monday,
            WeekAlternation.Both)]
        public void AddGsaClassAndDelete_ClassCreatedAndDeleted(
            string megaFacultyName,
            string gsaCourseName,
            int courseNumber,
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            DayOfWeek day,
            WeekAlternation week)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty(megaFacultyName);
            GsaCourse course = _gsaService.AddGsaCourse(megaFaculty, gsaCourseName, new CourseNumber(courseNumber));
            Room room = _isuService.AddRoom("room");
            Teacher teacher = _isuService.AddTeacher("teacher");
            var time = new TimeStamp(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            GsaClass gsaClass = _gsaService.AddGsaClass(course, time, teacher, room);
            Assert.AreEqual(gsaClass, teacher.TimeTable.FirstOrDefault());
            Assert.AreEqual(gsaClass, room.TimeTable.FirstOrDefault());
            _gsaService.DeleteGsaClass(gsaClass);
            Assert.IsEmpty(teacher.TimeTable);
            Assert.IsEmpty(room.TimeTable);
        } 
        
            [TestCase(
                10, 
                0, 
                11, 
                30, 
                DayOfWeek.Monday, 
                WeekAlternation.Both)]
        
        public void AddGsaClassesButRoomAndTeacherNotAvailable_ThrowException(
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            DayOfWeek day,
            WeekAlternation week)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            var time = new TimeStamp(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            Room room1 = _isuService.AddRoom("room1");
            Room room2 = _isuService.AddRoom("room2");
            Teacher teacher1 = _isuService.AddTeacher("teacher1");
            Teacher teacher2 = _isuService.AddTeacher("teacher2");
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty, "-", new CourseNumber(1));
            _gsaService.AddGsaClass(gsaCourse, time, teacher1, room1);
            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddGsaClass(gsaCourse, time, teacher1, room2);
            });
            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddGsaClass(gsaCourse, time, teacher2, room1);
            });
        }

        [TestCase(
            "Vasya",
            1,
            'M',
            "00",
            11,
            40,
            13,
            10,
            DayOfWeek.Friday,
            WeekAlternation.Odd)]
        public void SignStudentToGsa_SuccessfullySigned(
            string studentName, 
            int numberOfCourse,
            char facultyLetter,
            string endOfNameOfGroup,
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            DayOfWeek day,
            WeekAlternation week)
        {
            MegaFaculty megaFaculty1 = _facultyManager.CreateMegaFaculty("MegaFaculty1");
            MegaFaculty megaFaculty2 = _facultyManager.CreateMegaFaculty("MegaFaculty2");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty1, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));
            var groupName = new GroupName(faculty, course, endOfNameOfGroup);
            _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(_isuService.FindGroup(groupName), studentName);
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty2, "GSA", new CourseNumber(numberOfCourse));
            Room room = _isuService.AddRoom("room");
            Teacher teacher = _isuService.AddTeacher("teacher");
            var time = new TimeStamp(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            _gsaService.AddGsaClass(gsaCourse, time, teacher, room);
            _gsaService.AddStudentToGsaCourse(gsaCourse, student);
            Assert.AreEqual(student, _gsaService.GetStudentsFromGsaCourse(gsaCourse).FirstOrDefault());
        }
        
        [TestCase(
            1,
            'M',
            "00",
            11,
            40,
            13,
            10,
            DayOfWeek.Friday,
            WeekAlternation.Odd)]
        public void SignStudentToGsaButLimitReached_ThrowException(
            int numberOfCourse,
            char facultyLetter,
            string endOfNameOfGroup,
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            DayOfWeek day,
            WeekAlternation week)
        {
            MegaFaculty megaFaculty1 = _facultyManager.CreateMegaFaculty("MegaFaculty1");
            MegaFaculty megaFaculty2 = _facultyManager.CreateMegaFaculty("MegaFaculty2");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty1, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));
            var groupName = new GroupName(faculty, course, endOfNameOfGroup);
            _isuService.AddGroup(groupName);
            Student student1 = _isuService.AddStudent(_isuService.FindGroup(groupName), "student1");
            Student student2 = _isuService.AddStudent(_isuService.FindGroup(groupName), "student2");
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty2, "GSA", new CourseNumber(numberOfCourse), 1);
            Room room = _isuService.AddRoom("room");
            Teacher teacher = _isuService.AddTeacher("teacher");
            var time = new TimeStamp(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            _gsaService.AddGsaClass(gsaCourse, time, teacher, room);
            _gsaService.AddStudentToGsaCourse(gsaCourse, student1);
            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddStudentToGsaCourse(gsaCourse, student2);
            });
        }
        
        [TestCase(
            1,
            'M',
            "00",
            11,
            40,
            13,
            10,
            DayOfWeek.Friday,
            WeekAlternation.Odd)]
        public void SignStudentToGsaButClassesIntersects_ThrowException(
            int numberOfCourse,
            char facultyLetter,
            string endOfNameOfGroup,
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            DayOfWeek day,
            WeekAlternation week)
        {
            MegaFaculty megaFaculty1 = _facultyManager.CreateMegaFaculty("MegaFaculty1");
            MegaFaculty megaFaculty2 = _facultyManager.CreateMegaFaculty("MegaFaculty2");
            Faculty faculty = _facultyManager.CreateFaculty("FITP", megaFaculty1, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));
            Subject subject = _courseManager.AddSubjectOnCourse(course, "Programming");
            var time = new TimeStamp(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            Room room1 = _isuService.AddRoom("room1");
            Room room2 = _isuService.AddRoom("room2");
            Teacher teacher1 = _isuService.AddTeacher("teacher1");
            Teacher teacher2 = _isuService.AddTeacher("teacher2");
            var groupName = new GroupName(faculty, course, endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            _courseManager.AddGroupStudyClass(subject, time, group, teacher1, room1);
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty2, "GSA", new CourseNumber(numberOfCourse));
            _gsaService.AddGsaClass(gsaCourse, time, teacher2, room2);
            Student student = _isuService.AddStudent(group, "Student");
            
            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddStudentToGsaCourse(gsaCourse, student);
            });
        }
        
        [TestCase(
            "Sasha",
            1,
            'M',
            "00")]
        public void SignStudentToGsaOnHisMegaFaculty_ThrowException(
            string studentName, 
            int numberOfCourse,
            char facultyLetter,
            string endOfNameOfGroup)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));
            var groupName = new GroupName(faculty, course, endOfNameOfGroup);
            _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(_isuService.FindGroup(groupName), studentName);
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty, "GSA", new CourseNumber(numberOfCourse));
            
            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddStudentToGsaCourse(gsaCourse, student);
            });
        }
        
        [TestCase(
            "Petya",
            1,
            2,
            'M',
            "00")]
        public void SignStudentToGsaOnWrongCourse_ThrowException(
            string studentName, 
            int numberOfStudentCourse,
            int numberOfGsaCourse,
            char facultyLetter,
            string endOfNameOfGroup)
        {
            MegaFaculty megaFaculty1 = _facultyManager.CreateMegaFaculty("MegaFaculty1");
            MegaFaculty megaFaculty2 = _facultyManager.CreateMegaFaculty("MegaFaculty2");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty1, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfStudentCourse));
            var groupName = new GroupName(faculty, course, endOfNameOfGroup);
            _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(_isuService.FindGroup(groupName), studentName);
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty2, "GSA", new CourseNumber(numberOfGsaCourse));

            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddStudentToGsaCourse(gsaCourse, student);
            });
        }
        
        [TestCase(
            "Lena",
            3,
            'P',
            "31")]
        public void SignStudentTo3GsaCourses_ThrowException(
            string studentName, 
            int numberOfCourse,
            char facultyLetter,
            string endOfNameOfGroup)
        {
            MegaFaculty megaFaculty1 = _facultyManager.CreateMegaFaculty("MegaFaculty1");
            MegaFaculty megaFaculty2 = _facultyManager.CreateMegaFaculty("MegaFaculty2");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty1, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));
            var groupName = new GroupName(faculty, course, endOfNameOfGroup);
            _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(_isuService.FindGroup(groupName), studentName);
            GsaCourse gsaCourse1 = _gsaService.AddGsaCourse(megaFaculty2, "GSA1", new CourseNumber(numberOfCourse));
            GsaCourse gsaCourse2 = _gsaService.AddGsaCourse(megaFaculty2, "GSA2", new CourseNumber(numberOfCourse));
            GsaCourse gsaCourse3 = _gsaService.AddGsaCourse(megaFaculty2, "GSA3", new CourseNumber(numberOfCourse));
            _gsaService.AddStudentToGsaCourse(gsaCourse1, student);
            _gsaService.AddStudentToGsaCourse(gsaCourse2, student);

            Assert.Throws<IsuException>(() =>
            {
                _gsaService.AddStudentToGsaCourse(gsaCourse3, student);
            });
        }
    }
}