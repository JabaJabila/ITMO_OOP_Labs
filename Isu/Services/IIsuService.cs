using System.Collections.Generic;
using Isu.DataTypes;
using Isu.Entities;
using Isu.Models;

namespace Isu.Services
{
    public interface IIsuService
    {
        Group AddGroup(GroupName name);
        Student AddStudent(Group group, string name);

        Student GetStudent(int id);
        Student FindStudent(string name);
        IReadOnlyList<Student> FindStudents(GroupName groupName);
        IReadOnlyList<Student> FindStudents(CourseNumber course);

        Group FindGroup(GroupName groupName);
        IReadOnlyList<Group> FindGroups(CourseNumber course);

        void ChangeStudentGroup(Student student, Group newGroup);
    }
}