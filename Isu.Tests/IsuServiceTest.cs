using System.Linq;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    [TestFixture]
    public class Tests
    {
        private IIsuService _isuService;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService(groupCapacity: 5);
        }

        [TestCase("Some Dude", 1, 'M', "00")]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent(
            string studentName, 
            int numberOfCourse,
            char facultyLetter,
            string endOfNameOfGroup)
        {
            var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup);
            _isuService.AddGroup(groupName);
            _isuService.AddStudent(_isuService.FindGroup(groupName), studentName);
            Assert.AreEqual(groupName.Name, _isuService.FindStudent(studentName).Group.GroupName.Name);
            Assert.IsTrue(_isuService.FindGroup(groupName).Students.Contains(_isuService.FindStudent(studentName)));
        }

        [TestCase(1, 'M', "00")]
        public void ReachMaxStudentPerGroup_ThrowException(
            int courseNumber,
            char facultyLetter,
            string endOfNameOfGroup)
        {
            var groupName = new GroupName(facultyLetter, new CourseNumber(courseNumber), "00");
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
            Assert.Throws<IsuException>(() =>
            {
                var groupName = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup);
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
            var groupName1 = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup1);
            var groupName2 = new GroupName(facultyLetter, new CourseNumber(numberOfCourse), endOfNameOfGroup2);
            _isuService.AddGroup(groupName1);
            _isuService.AddGroup(groupName2);
            Student student = _isuService.AddStudent(_isuService.FindGroup(groupName1), studentName);
            _isuService.ChangeStudentGroup(student, _isuService.FindGroup(groupName2));

            Assert.AreEqual(groupName2.Name, _isuService.FindStudent(studentName).Group.GroupName.Name);
        }
    }
}