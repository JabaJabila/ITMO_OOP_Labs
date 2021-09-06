using System.Collections.Generic;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private readonly uint _groupCapacity;

        public Group(string groupName, uint groupCapacity)
        {
            GroupName = groupName;
            Students = new List<Student>();
            _groupCapacity = groupCapacity;
        }

        public string GroupName { get; }

        public List<Student> Students { get; }

        internal static bool CheckGroupName(string groupName)
        {
            if (!(groupName.Length == 5 && groupName.Substring(0, 2) == "M3"))
                return false;

            if (groupName[2] < '0' || groupName[2] > '4') return false;
            for (int i = 0; i < 2; ++i)
            {
                if (groupName[3 + i] >= '0' && groupName[3 + i] <= '9')
                    return true;
            }

            return false;
        }

        internal void AddStudentToGroup(Student student)
        {
            if (Students.Count == _groupCapacity)
                throw new IsuException("Group reached limit of students!");
            if (student.Group != null)
                throw new IsuException("Student is already has group!");
            Students.Add(student);
        }

        internal void DeleteStudentFromGroup(Student student)
        {
            if (!Students.Remove(student))
                throw new IsuException("Student wasn't in this group!");
        }
    }
}
