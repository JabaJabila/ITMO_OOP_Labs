using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Subject
    {
        private readonly List<GroupStudyClass> _classes;

        internal Subject(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            _classes = new List<GroupStudyClass>();
        }

        public string Name { get; }
        public IReadOnlyCollection<GroupStudyClass> StudyClasses => _classes;

        public GroupStudyClass AddGroupStudyClass(
            StudyStreamPeriod studyStreamPeriod,
            Group group,
            Teacher teacher,
            Room room)
        {
            if (studyStreamPeriod == null)
                throw new ArgumentNullException(nameof(studyStreamPeriod));

            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));

            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (teacher.TimeTable.Any(studyStream =>
                studyStream.StudyStreamPeriod.CheckIfIntersects(studyStreamPeriod)))
            {
                throw new IsuException($"Teacher {teacher.Name} has classes " +
                                       $"that intersects with this timestamp!");
            }

            if (room.TimeTable.Any(studyStream =>
                studyStream.StudyStreamPeriod.CheckIfIntersects(studyStreamPeriod)))
            {
                throw new IsuException($"Room {room.Number} is used for classes " +
                                       $"that intersects with this timestamp!");
            }

            if (_classes
                .Where(otherStudyClass => otherStudyClass.Group.GroupName.Name == group.GroupName.Name)
                .Any(otherStudyClass => otherStudyClass.StudyStreamPeriod.CheckIfIntersects(studyStreamPeriod)))
            {
                throw new IsuException($"Group {@group.GroupName.Name} has classes " +
                                       $"that intersects with this timestamp!");
            }

            var studyClass = new GroupStudyClass(this, studyStreamPeriod, group, teacher, room);

            _classes.Add(studyClass);
            return studyClass;
        }

        public void DeleteStudyClass(GroupStudyClass groupStudyClass)
        {
            if (groupStudyClass == null)
                throw new ArgumentNullException(nameof(groupStudyClass));

            if (!_classes.Remove(groupStudyClass))
                throw new IsuException("GsaClass not on this GsaCourse!");

            groupStudyClass.Delete();
        }

        internal void Delete()
        {
            _classes.ForEach(DeleteStudyClass);
        }
    }
}