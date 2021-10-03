using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Entities;
using Isu.Tools;
using IsuExtra.Models;

namespace IsuExtra.Entities
{
    public class Subject
    {
        private readonly List<GroupStudyClass> _classes;

        internal Subject(string name)
        {
            Name = name ?? throw new ArgumentNullException(
                    nameof(name),
                    $"{nameof(name)} can't be null!");

            _classes = new List<GroupStudyClass>();
        }

        public string Name { get; }
        public IReadOnlyCollection<GroupStudyClass> StudyClasses => _classes;

        internal GroupStudyClass AddStudyClass(TimeStamp timeStamp, Group group,  Teacher teacher, Room room)
        {
            if (teacher.TimeTable.Any(studyStream => studyStream.TimeStamp.CheckIfIntersects(timeStamp)))
            {
                throw new IsuException($"Teacher {teacher.Name} has classes " +
                                       $"that intersects with this timestamp!");
            }

            if (room.TimeTable.Any(studyStream => studyStream.TimeStamp.CheckIfIntersects(timeStamp)))
            {
                throw new IsuException($"Room {room.Number} is used for classes " +
                                       $"that intersects with this timestamp!");
            }

            if (_classes
                .Where(otherStudyClass => otherStudyClass.Group.GroupName.Name == group.GroupName.Name)
                .Any(otherStudyClass => otherStudyClass.TimeStamp.CheckIfIntersects(timeStamp)))
            {
                throw new IsuException($"Group {@group.GroupName.Name} has classes " +
                                       $"that intersects with this timestamp!");
            }

            var studyClass = new GroupStudyClass(this, timeStamp, group, teacher, room);

            _classes.Add(studyClass);
            return studyClass;
        }

        internal void DeleteStudyClass(GroupStudyClass groupStudyClass)
        {
            if (groupStudyClass == null)
            {
                throw new ArgumentNullException(
                    nameof(groupStudyClass),
                    $"{nameof(groupStudyClass)} can't be null!");
            }

            if (!_classes.Remove(groupStudyClass))
                throw new IsuException("GsaClass not on this GsaCourse!");

            groupStudyClass.DeleteGroupStudyClass();
        }

        internal void Delete()
        {
            _classes.ForEach(DeleteStudyClass);
        }
    }
}