using System;

namespace IsuExtra.Entities
{
    public class GsaClass : StudyStream
    {
        internal GsaClass(
            GsaCourse gsaCourse,
            StudyStreamPeriod studyStreamPeriod,
            Teacher teacher,
            Room room)
            : base(studyStreamPeriod, teacher, room)
        {
            GsaCourse = gsaCourse ?? throw new ArgumentNullException(nameof(gsaCourse));
        }

        public GsaCourse GsaCourse { get; }
    }
}