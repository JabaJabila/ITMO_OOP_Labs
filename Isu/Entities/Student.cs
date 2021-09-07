using Isu.Tools;

namespace Isu.Entities
{
    public class Student
    {
        public Student(string name, int id)
        {
            Name = name ?? throw new IsuException("Student must have name!");
            Id = id;
            Group = null;
        }

        public int Id { get; }

        public string Name { get; }

        public Group Group { get; private set; }

        public void ChangeGroup(Group newGroup)
        {
            if (Group != null)
            {
                Group.DeleteStudentFromGroup(this);
                Group = null;
            }

            if (newGroup != null)
            {
                newGroup.AddStudentToGroup(this);
                Group = newGroup;
            }
        }
    }
}