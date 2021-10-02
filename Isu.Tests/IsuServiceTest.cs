using System;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Services;
using Isu.Models;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    [TestFixture]
    public class Tests
    {
        private IsuService _isuService;
        private FacultyManager _facultyManager;
        private CourseManager _courseManager;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService(groupCapacity: 5);
            _facultyManager = new FacultyManager();
            _courseManager = new CourseManager();
        }

        [TestCase("TINT", "FITP", 'M')]
        public void AddMegaFacultyAndFaculty_MegaFacultyHasFaculty(
            string megaFacultyName,
            string facultyName, 
            char letter)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty(megaFacultyName);
            Faculty faculty = _facultyManager.CreateFaculty(facultyName, megaFaculty, letter);
            Assert.AreEqual(faculty, megaFaculty.Faculties.FirstOrDefault());
        }

        [TestCase("TINT", "FITP", '1')]
        public void AddMegaFacultyAndFacultyWithInvalidLetter_ThrowException(
            string megaFacultyName,
            string facultyName,
            char letter)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty(megaFacultyName);
            Assert.Throws<IsuException>(() =>
            {
                _facultyManager.CreateFaculty(facultyName, megaFaculty, letter);
            });
        }

        [TestCase("TINT", "FITP", "IKT", 'M')]
        public void AddMegaFacultyAndFacultyWithExistedLetter_ThrowException(
            string megaFacultyName,
            string facultyName1,
            string facultyName2,
            char letter)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty(megaFacultyName);
            _facultyManager.CreateFaculty(facultyName1, megaFaculty, letter);
            Assert.Throws<IsuException>(() =>
            {
                _facultyManager.CreateFaculty(facultyName2, megaFaculty, letter);
            });
        }

        [TestCase("Some Dude", 1, 'M', "00")]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent(
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
            _isuService.AddStudent(_isuService.FindGroup(groupName), studentName);
            Assert.AreEqual(groupName.Name, _isuService.FindStudent(studentName).Group.GroupName.Name);
            Assert.IsTrue(_isuService.FindGroup(groupName).Students.Contains(_isuService.FindStudent(studentName)));
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty, 'A');
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(1));
            var groupName = new GroupName(faculty, course, "00");
            _isuService.AddGroup(groupName);
            _isuService.AddStudent(_isuService.FindGroup(groupName), "Student1");
            _isuService.AddStudent(_isuService.FindGroup(groupName), "Student2");
            _isuService.AddStudent(_isuService.FindGroup(groupName), "Student3");
            _isuService.AddStudent(_isuService.FindGroup(groupName), "Student4");
            _isuService.AddStudent(_isuService.FindGroup(groupName), "Student5");

            Assert.Throws<IsuException>(() =>
            {
                _isuService.AddStudent(_isuService.FindGroup(groupName), "Student6");
            });
        }

        [TestCase('A', 1, "0")]
        [TestCase('B', 2, "a")]
        public void CreateGroupWithInvalidName_ThrowException(
            char facultyLetter, 
            int numberOfCourse, 
            string endOfNameOfGroup)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));

            Assert.Throws<IsuException>(() =>
            {
                var groupName = new GroupName(faculty, course, endOfNameOfGroup);
                _isuService.AddGroup(groupName);
            });

        }

        [TestCase("Vasya", 'S', 3, "01", "01c")]
        public void TransferStudentToAnotherGroup_GroupChanged(
            string studentName,
            char facultyLetter, 
            int numberOfCourse, 
            string endOfNameOfGroup1,
            string endOfNameOfGroup2)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            Faculty faculty = _facultyManager.CreateFaculty("Faculty", megaFaculty, facultyLetter);
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(numberOfCourse));
            var groupName1 = new GroupName(faculty, course, endOfNameOfGroup1);
            var groupName2 = new GroupName(faculty, course, endOfNameOfGroup2);
            _isuService.AddGroup(groupName1);
            _isuService.AddGroup(groupName2);
            Student student = _isuService.AddStudent(_isuService.FindGroup(groupName1), studentName);
            _isuService.ChangeStudentGroup(student, _isuService.FindGroup(groupName2));
            
            
            Assert.AreEqual(groupName2.Name, _isuService.FindStudent(studentName).Group.GroupName.Name);
        }

        [TestCase(
            12,
            0,
            13,
            0,
            12,
            45,
            13,
            50,
            DayOfWeek.Monday,
            WeekAlternation.Even,
            WeekAlternation.Both)]
        [TestCase(
            10,
            0,
            13, 
            0,
            12,
            0,
            12,
            50, 
            DayOfWeek.Wednesday,
            WeekAlternation.Even,
            WeekAlternation.Even)]
        [TestCase(
            15,
            0, 
            16, 
            0, 
            13, 
            45, 
            15, 
            50, 
            DayOfWeek.Saturday,
            WeekAlternation.Odd,
            WeekAlternation.Both)]
        [TestCase(
            12, 
            0, 
            13, 
            0, 
            11, 
            45, 
            13, 
            50, 
            DayOfWeek.Friday,
            WeekAlternation.Odd,
            WeekAlternation.Odd)]
        public void TwoTimeStampsAreIntersects_ReturnsTrue(
            int hoursBegin1,
            int minutesBegin1,
            int hoursEnd1,
            int minutesEnd1,
            int hoursBegin2,
            int minutesBegin2,
            int hoursEnd2,
            int minutesEnd2,
            DayOfWeek day,
            WeekAlternation week1,
            WeekAlternation week2)
        {
            var time1 = new TimeStamp(new TimeSpan(hoursBegin1, minutesBegin1, 0),
                new TimeSpan(hoursEnd1, minutesEnd1, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week1);
            var time2 = new TimeStamp(new TimeSpan(hoursBegin2, minutesBegin2, 0),
                new TimeSpan(hoursEnd2, minutesEnd2, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week2);
            Assert.IsTrue(time1.CheckIfIntersects(time2));
        }

        [TestCase(
            12, 
            0, 
            13, 
            0, 
            13, 
            45, 
            14, 
            50, 
            DayOfWeek.Monday, 
            DayOfWeek.Monday,
            WeekAlternation.Even,
            WeekAlternation.Both)]
        [TestCase(12, 
            0, 
            13, 
            0, 
            12, 
            15, 
            12, 
            50, 
            DayOfWeek.Thursday, 
            DayOfWeek.Sunday,
            WeekAlternation.Both,
            WeekAlternation.Odd)]
        [TestCase(
            12, 
            0, 
            13, 
            0, 
            11, 
            45, 
            13, 
            50, 
            DayOfWeek.Friday,
            DayOfWeek.Friday,
            WeekAlternation.Odd,
            WeekAlternation.Even)]
        public void TwoTimeStampsNotIntersects_ReturnsFalse(int hoursBegin1,
            int minutesBegin1,
            int hoursEnd1,
            int minutesEnd1,
            int hoursBegin2,
            int minutesBegin2,
            int hoursEnd2,
            int minutesEnd2,
            DayOfWeek day1,
            DayOfWeek day2,
            WeekAlternation week1,
            WeekAlternation week2)
        {
            var time1 = new TimeStamp(new TimeSpan(hoursBegin1, minutesBegin1, 0),
                new TimeSpan(hoursEnd1, minutesEnd1, 0),
                day1,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week1);
            var time2 = new TimeStamp(new TimeSpan(hoursBegin2, minutesBegin2, 0),
                new TimeSpan(hoursEnd2, minutesEnd2, 0), 
                day2,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week2);
            Assert.IsFalse(time1.CheckIfIntersects(time2));
        }
        
        [TestCase(
            12, 
            0, 
            11, 
            0, 
            2020, 
            2020, 
            11, 
            12, 
            10, 
            10, 
            DayOfWeek.Friday)]
        [TestCase(22, 
            0, 
            13, 
            40, 
            2020, 
            2020, 
            10, 
            11, 
            1, 
            2, 
            DayOfWeek.Monday)]
        [TestCase(16, 
            0, 
            15, 
            59, 
            2020, 
            2020, 
            11, 
            12, 
            1, 
            10, 
            DayOfWeek.Tuesday)]
        [TestCase(16, 
            0, 
            17, 
            00, 
            2020, 
            2020, 
            11, 
            11, 
            10, 
            1, 
            DayOfWeek.Tuesday)]
        public void InvalidTimeStamp_ThrowsException(
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            int year1,
            int year2,
            int month1,
            int month2,
            int day1,
            int day2,
            DayOfWeek day)
        {
            Assert.Throws<IsuException>(() =>
            {
                new TimeStamp(
                    new TimeSpan(hoursBegin, minutesBegin, 0), 
                    new TimeSpan(hoursEnd, minutesEnd, 0), 
                    day,
                    new DateTime(year1, month1, day1),
                    new DateTime(year2, month2, day2));
            });
        }

        [TestCase("programming", 1, 10, 0, 11, 30, DayOfWeek.Monday)]
        public void AddSubjectAndStudyClassesAndDeleted_SuccessfullyAddedAndDeleted(
            string subjectName,
            int courseNumber,
            int hoursBegin,
            int minutesBegin,
            int hoursEnd,
            int minutesEnd,
            DayOfWeek day)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            Faculty faculty = _facultyManager.CreateFaculty("FITP", megaFaculty, 'M');
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(1));
            Subject subject = _courseManager.AddSubjectOnCourse(course, "Programming");
            var time = new TimeStamp(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0));
            Room room = _isuService.AddRoom("room");
            Teacher teacher = _isuService.AddTeacher("teacher");
            var groupName = new GroupName(faculty, course, "00");
            Group group = _isuService.AddGroup(groupName);
            GroupStudyClass groupStudyClass = _courseManager.AddGroupStudyClass(subject, time, group, teacher, room);
            Assert.AreEqual(groupStudyClass, group.TimeTable.FirstOrDefault());
            Assert.AreEqual(groupStudyClass, teacher.TimeTable.FirstOrDefault());
            Assert.AreEqual(groupStudyClass, room.TimeTable.FirstOrDefault());
            _courseManager.DeleteStudyClass(subject, groupStudyClass);
            Assert.IsEmpty(group.TimeTable);
            Assert.IsEmpty(teacher.TimeTable);
            Assert.IsEmpty(room.TimeTable);
            _courseManager.DeleteSubjectFromCourse(course, subject);
            Assert.IsEmpty(course.Subjects);
        }

        [TestCase(
            "programming",
            1,
            10,
            0,
            11,
            30,
            10,
            30,
            12,
            0,
            DayOfWeek.Monday)]
        public void StudyClassesIntersect_ThrowException(
            string subjectName,
            int courseNumber,
            int hoursBegin1,
            int minutesBegin1,
            int hoursEnd1,
            int minutesEnd1,
            int hoursBegin2,
            int minutesBegin2,
            int hoursEnd2,
            int minutesEnd2,
            DayOfWeek day)
        {
            MegaFaculty megaFaculty = _facultyManager.CreateMegaFaculty("MegaFaculty");
            Faculty faculty = _facultyManager.CreateFaculty("FITP", megaFaculty, 'M');
            Course course = _facultyManager.AddCourseOnFaculty(faculty, new CourseNumber(1));
            Subject subject = _courseManager.AddSubjectOnCourse(course, "Programming");
            var time1 = new TimeStamp(
                new TimeSpan(hoursBegin1, minutesBegin1, 0),
                new TimeSpan(hoursEnd1, minutesEnd1, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0));
            var time2 = new TimeStamp(
                new TimeSpan(hoursBegin2, minutesBegin2, 0),
                new TimeSpan(hoursEnd2, minutesEnd2, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0));
            Room room1 = _isuService.AddRoom("room1");
            Room room2 = _isuService.AddRoom("room2");
            Teacher teacher1 = _isuService.AddTeacher("teacher1");
            Teacher teacher2 = _isuService.AddTeacher("teacher2");
            var groupName1 = new GroupName(faculty, course, "00");
            var groupName2 = new GroupName(faculty, course, "01");
            Group group1 = _isuService.AddGroup(groupName1);
            Group group2 = _isuService.AddGroup(groupName2);
            _courseManager.AddGroupStudyClass(subject, time1, group1, teacher1, room1);
            Assert.Throws<IsuException>(() =>
            {
                _courseManager.AddGroupStudyClass(subject, time2, group1, teacher1, room1);
            });
            Assert.Throws<IsuException>(() =>
            {
                _courseManager.AddGroupStudyClass(subject, time2, group2, teacher2, room1);
            });
            Assert.Throws<IsuException>(() =>
            {
                _courseManager.AddGroupStudyClass(subject, time2, group2, teacher1, room2);
            });
            Assert.Throws<IsuException>(() =>
            {
                _courseManager.AddGroupStudyClass(subject, time2, group1, teacher2, room2);
            });
        }
    }
}