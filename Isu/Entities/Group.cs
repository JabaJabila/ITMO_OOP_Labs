using System.Collections.Generic;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private readonly uint _groupCapacity;
        private readonly string _groupName;
        private readonly List<Student> _students;

        public Group(string groupName, uint groupCapacity)
        {
            _groupName = groupName;
            _students = new List<Student>();
            _groupCapacity = groupCapacity;
        }

        public string GroupName
        {
            get => _groupName;
        }

        public List<Student> Students
        {
            get => _students;
        }

        internal static bool CheckGroupName(string groupName) // static because it checks name before creating instance
        {
            if (!(groupName.Length == 5 && groupName.Substring(0, 2) == "M3"))
                return false;

            if (groupName[2] >= '0' && groupName[2] <= '4')
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (groupName[3 + i] >= '0' && groupName[3 + i] <= '9')
                        return true;
                }
            }

            return false;
        }

        internal void AddStudentToGroup(Student student)
        {
            if (_students.Count == _groupCapacity)
                throw new IsuException("Group reached limit of students!");
            else if (student.Group != null)
                throw new IsuException("Student is already has group!");
            else _students.Add(student);
        }

        internal void DeleteStudentFromGroup(Student student)
        {
            if (_students.Remove(student))
            {
                return;
            }

            throw new IsuException("Student wasn't in this group!");
        }
    }
}
