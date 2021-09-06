using System.Collections.Generic;
using Isu.Tools;

namespace Isu.Entities
{
    public class Course
    {
        private readonly int _number;
        private readonly List<Group> _groups;

        public Course(int number)
        {
            if (number is >= 1 and <= 4)
                _number = number;
            else throw new IsuException("Invalid Number!\n Number must be in range[1..4]");
            _groups = new List<Group>();
        }

        public int Number
        {
            get => _number;
        }

        public List<Group> Groups
        {
            get => _groups;
        }

        public void AddGroupToCourse(Group group)
        {
            _groups.Add(group);
        }
    }
}