using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class Teacher
    {
        private readonly List<StudyStream> _timeTable;

        internal Teacher(string name, int id)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            Id = id;
            _timeTable = new List<StudyStream>();
        }

        public string Name { get; }
        public int Id { get; }
        public IReadOnlyCollection<StudyStream> TimeTable => _timeTable;

        public void AddStudyClassToTimeTable(StudyStream studyStream)
        {
            if (studyStream == null)
                throw new ArgumentNullException(nameof(studyStream));

            if (_timeTable.Any(existingClass => studyStream.StudyStreamPeriod.CheckIfIntersects(existingClass.StudyStreamPeriod)))
                throw new IsuException($"StudyClass intersects with teacher {Name}'s TimeTable!");

            _timeTable.Add(studyStream);
        }

        internal void RemoveStudyClassFromTimeTable(StudyStream studyStream)
        {
            if (studyStream == null)
                throw new ArgumentNullException(nameof(studyStream));

            _timeTable.Remove(studyStream);
        }
    }
}