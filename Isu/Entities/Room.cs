using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Tools;

namespace Isu.Entities
{
    public class Room
    {
        private readonly List<StudyStream> _timeTable;

        internal Room(string number)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number), $"{nameof(number)} can't be null!");
            _timeTable = new List<StudyStream>();
        }

        public string Number { get; }
        public IReadOnlyCollection<StudyStream> TimeTable => _timeTable;

        internal void AddStudyClassToTimeTable(StudyStream studyStream)
        {
            if (studyStream == null)
                throw new ArgumentNullException(nameof(studyStream), $"{nameof(studyStream)} can't be null!");

            if (_timeTable.Any(existingClass => studyStream.TimeStamp.CheckIfIntersects(existingClass.TimeStamp)))
                throw new IsuException($"Room {Number} is unavailable for this StudyClass!");

            _timeTable.Add(studyStream);
        }

        internal void RemoveStudyClassFromTimeTable(StudyStream studyStream)
        {
            if (studyStream == null)
                throw new ArgumentNullException(nameof(studyStream), $"{nameof(studyStream)} can't be null!");

            _timeTable.Remove(studyStream);
        }
    }
}