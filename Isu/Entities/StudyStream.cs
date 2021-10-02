using System;
using Isu.Models;

namespace Isu.Entities
{
    public class StudyStream
    {
        protected StudyStream(
            TimeStamp timeStamp,
            Teacher teacher,
            Room room)
        {
            TimeStamp = timeStamp ?? throw new ArgumentNullException(
                nameof(timeStamp),
                $"{nameof(timeStamp)} can't be null!");

            Teacher = teacher ?? throw new ArgumentNullException(
                nameof(teacher),
                $"{nameof(teacher)} can't be null!");

            Room = room ?? throw new ArgumentNullException(
                nameof(room),
                $"{nameof(room)} can't be null!");

            room.AddStudyClassToTimeTable(this);
            teacher.AddStudyClassToTimeTable(this);
        }

        public TimeStamp TimeStamp { get; }
        public Teacher Teacher { get; }
        public Room Room { get; }

        protected void Delete()
        {
            Teacher.RemoveStudyClassFromTimeTable(this);
            Room.RemoveStudyClassFromTimeTable(this);
        }
    }
}