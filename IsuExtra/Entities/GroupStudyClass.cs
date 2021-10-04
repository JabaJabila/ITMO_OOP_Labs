using System;
using Isu.Entities;

namespace IsuExtra.Entities
{
    public class GroupStudyClass : StudyStream
    {
        internal GroupStudyClass(
            Subject subject,
            StudyStreamPeriod studyStreamPeriod,
            Group group,
            Teacher teacher,
            Room room)
            : base(studyStreamPeriod, teacher, room)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));

            Group = group ?? throw new ArgumentNullException(nameof(group));
        }

        public Group Group { get; }
        public Subject Subject { get; }
    }
}