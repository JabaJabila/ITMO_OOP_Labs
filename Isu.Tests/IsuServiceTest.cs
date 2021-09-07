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
            _isuService.AddGroup("M3101");
            _isuService.AddGroup("M3102");
            _isuService.AddGroup("M3200");
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            _isuService.AddStudent(_isuService.FindGroup("M3101"), "Vasya Pupkin");
            Assert.AreEqual(_isuService.FindStudent("Vasya Pupkin").Group, _isuService.FindGroup("M3101"));
            Assert.IsTrue(_isuService.FindGroup("M3101").Students.Contains(_isuService.FindStudent("Vasya Pupkin")));
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Assert.Throws<IsuException>(() =>
            {
                _isuService.AddStudent(_isuService.FindGroup("M3200"), "Andreev Artem");
                _isuService.AddStudent(_isuService.FindGroup("M3200"), "Georgy Kruglov");
                _isuService.AddStudent(_isuService.FindGroup("M3200"), "Mihail Kutuzov");
                _isuService.AddStudent(_isuService.FindGroup("M3200"), "Saratovcev Edgar");
                _isuService.AddStudent(_isuService.FindGroup("M3200"), "Rakhmankulov Aedgar");
                _isuService.AddStudent(_isuService.FindGroup("M3200"), "Ivan Evtushenko");
            });
        }

        [Test]
        public void CreateGroupWithInvalidName_ThrowException()
        {
            Assert.Throws<IsuException>(() =>
            {
                _isuService.AddGroup("M30aa");
            });

        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            _isuService.AddStudent(_isuService.FindGroup("M3101"), "Noname Student");
            _isuService.ChangeStudentGroup(_isuService.FindStudent("Noname Student"), _isuService.FindGroup("M3102"));
            Assert.AreEqual(_isuService.FindStudent("Noname Student").Group, _isuService.FindGroup("M3102"));
        }
    }
}