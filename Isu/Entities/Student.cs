using System;

namespace Isu.Entities
{
    public class Student
    {
        internal Student(string name, int id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

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

            if (newGroup == null) return;
            newGroup.AddStudentToGroup(this);
            Group = newGroup;
        }
    }
}