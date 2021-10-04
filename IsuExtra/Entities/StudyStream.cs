using System;

namespace IsuExtra.Entities
{
    public class StudyStream
    {
        protected StudyStream(
            StudyStreamPeriod studyStreamPeriod,
            Teacher teacher,
            Room room)
        {
            StudyStreamPeriod = studyStreamPeriod ?? throw new ArgumentNullException(nameof(studyStreamPeriod));

            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));

            Room = room ?? throw new ArgumentNullException(nameof(room));

            room.AddStudyClassToTimeTable(this);
            teacher.AddStudyClassToTimeTable(this);
        }

        public StudyStreamPeriod StudyStreamPeriod { get; }
        public Teacher Teacher { get; }
        public Room Room { get; }

        internal void Delete()
        {
            Teacher.RemoveStudyClassFromTimeTable(this);
            Room.RemoveStudyClassFromTimeTable(this);
        }
    }
}