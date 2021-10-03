using System;
using IsuExtra.Models;

namespace IsuExtra.Entities
{
    public class GsaClass : StudyStream
    {
        internal GsaClass(
            GsaCourse gsaCourse,
            TimeStamp timeStamp,
            Teacher teacher,
            Room room)
            : base(timeStamp, teacher, room)
        {
            GsaCourse = gsaCourse ?? throw new ArgumentNullException(
                nameof(gsaCourse),
                $"{nameof(gsaCourse)} can't be null!");
        }

        public GsaCourse GsaCourse { get; }
    }
}