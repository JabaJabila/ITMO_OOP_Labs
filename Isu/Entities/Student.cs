using Isu.Tools;

namespace Isu.Entities
{
    public class Student
    {
        private readonly string _name;
        private readonly int _id;
        private Group _group;

        public Student(string name, int id)
        {
            _name = name ?? throw new IsuException("Student must have name!");
            _id = id;
            _group = null;
        }

        public int Id
        {
            get => _id;
        }

        public string Name
        {
            get => _name;
        }

        public Group Group
        {
            get => _group;
        }

        public void ChangeGroup(Group newGroup)
        {
            if (_group != null)
            {
                _group.DeleteStudentFromGroup(this);
                _group = null;
            }

            if (newGroup != null)
            {
                newGroup.AddStudentToGroup(this);
                _group = newGroup;
            }
        }
    }
}