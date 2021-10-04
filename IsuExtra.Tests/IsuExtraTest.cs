using System;
using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
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
        private StudyProcessManager _studyProcessManager;
        private FacultyManager _facultyManager;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService(groupCapacity: 5);
            _facultyManager = new FacultyManager();
            _gsaService = new GsaService(_facultyManager);
            _studyProcessManager = new StudyProcessManager();
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
            var time1 = new StudyStreamPeriod(new TimeSpan(hoursBegin1, minutesBegin1, 0),
                new TimeSpan(hoursEnd1, minutesEnd1, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week1);
            var time2 = new StudyStreamPeriod(new TimeSpan(hoursBegin2, minutesBegin2, 0),
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
            var time1 = new StudyStreamPeriod(new TimeSpan(hoursBegin1, minutesBegin1, 0),
                new TimeSpan(hoursEnd1, minutesEnd1, 0),
                day1,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week1);
            var time2 = new StudyStreamPeriod(new TimeSpan(hoursBegin2, minutesBegin2, 0),
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
                new StudyStreamPeriod(
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
            Subject subject = course.AddSubjectToCourse("Programming");
            var time = new StudyStreamPeriod(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0));
            Room room = _studyProcessManager.AddRoom("room");
            Teacher teacher = _studyProcessManager.AddTeacher("teacher");
            var groupName = new GroupName('M', new CourseNumber(courseNumber), "00");
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
            GroupStudyClass groupStudyClass = subject.AddGroupStudyClass(time, group, teacher, room);
            Assert.AreEqual(groupStudyClass, subject.StudyClasses.FirstOrDefault());
            Assert.AreEqual(groupStudyClass, teacher.TimeTable.FirstOrDefault());
            Assert.AreEqual(groupStudyClass, room.TimeTable.FirstOrDefault());
            subject.DeleteStudyClass(groupStudyClass);
            Assert.IsEmpty(subject.StudyClasses);
            Assert.IsEmpty(teacher.TimeTable);
            Assert.IsEmpty(room.TimeTable);
            course.DeleteSubjectFromCourse(subject);
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
            Subject subject = course.AddSubjectToCourse("Programming");
            var time1 = new StudyStreamPeriod(
                new TimeSpan(hoursBegin1, minutesBegin1, 0),
                new TimeSpan(hoursEnd1, minutesEnd1, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0));
            var time2 = new StudyStreamPeriod(
                new TimeSpan(hoursBegin2, minutesBegin2, 0),
                new TimeSpan(hoursEnd2, minutesEnd2, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0));
            Room room1 = _studyProcessManager.AddRoom("room1");
            Room room2 = _studyProcessManager.AddRoom("room2");
            Teacher teacher1 = _studyProcessManager.AddTeacher("teacher1");
            Teacher teacher2 = _studyProcessManager.AddTeacher("teacher2");
            var groupName1 = new GroupName('M', new CourseNumber(courseNumber), "00");
            var groupName2 = new GroupName('M', new CourseNumber(courseNumber), "01");
            Group group1 = _isuService.AddGroup(groupName1);
            Group group2 = _isuService.AddGroup(groupName2);
            course.AddGroupToCourse(group1);
            course.AddGroupToCourse(group2);
            subject.AddGroupStudyClass(time1, group1, teacher1, room1);
            Assert.Throws<IsuException>(() =>
            {
                subject.AddGroupStudyClass(time2, group1, teacher1, room1);
            });
            Assert.Throws<IsuException>(() =>
            {
                subject.AddGroupStudyClass(time2, group2, teacher2, room1);
            });
            Assert.Throws<IsuException>(() =>
            {
                subject.AddGroupStudyClass(time2, group2, teacher1, room2);
            });
            Assert.Throws<IsuException>(() =>
            {
                subject.AddGroupStudyClass(time2, group1, teacher2, room2);
            });
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
            Room room = _studyProcessManager.AddRoom("room");
            Teacher teacher = _studyProcessManager.AddTeacher("teacher");
            var time = new StudyStreamPeriod(
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
            var time = new StudyStreamPeriod(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            Room room1 = _studyProcessManager.AddRoom("room1");
            Room room2 = _studyProcessManager.AddRoom("room2");
            Teacher teacher1 = _studyProcessManager.AddTeacher("teacher1");
            Teacher teacher2 = _studyProcessManager.AddTeacher("teacher2");
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
            var groupName = new GroupName(facultyLetter,new CourseNumber(numberOfCourse), endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
            Student student = _isuService.AddStudent(group, studentName);
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty2, "GSA", new CourseNumber(numberOfCourse));
            Room room = _studyProcessManager.AddRoom("room");
            Teacher teacher = _studyProcessManager.AddTeacher("teacher");
            var time = new StudyStreamPeriod(
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
            var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
            Student student1 = _isuService.AddStudent(_isuService.FindGroup(groupName), "student1");
            Student student2 = _isuService.AddStudent(_isuService.FindGroup(groupName), "student2");
            GsaCourse gsaCourse = _gsaService.AddGsaCourse(megaFaculty2, "GSA", new CourseNumber(numberOfCourse), 1);
            Room room = _studyProcessManager.AddRoom("room");
            Teacher teacher = _studyProcessManager.AddTeacher("teacher");
            var time = new StudyStreamPeriod(
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
            Subject subject = course.AddSubjectToCourse("Programming");
            var time = new StudyStreamPeriod(
                new TimeSpan(hoursBegin, minutesBegin, 0),
                new TimeSpan(hoursEnd, minutesEnd, 0),
                day,
                DateTime.Now,
                DateTime.Now + new TimeSpan(365, 0, 0),
                week);
            Room room1 = _studyProcessManager.AddRoom("room1");
            Room room2 = _studyProcessManager.AddRoom("room2");
            Teacher teacher1 = _studyProcessManager.AddTeacher("teacher1");
            Teacher teacher2 = _studyProcessManager.AddTeacher("teacher2");
            var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
            subject.AddGroupStudyClass(time, group, teacher1, room1);
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
            var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
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
            var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfStudentCourse), endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
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
            var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup);
            Group group = _isuService.AddGroup(groupName);
            course.AddGroupToCourse(group);
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