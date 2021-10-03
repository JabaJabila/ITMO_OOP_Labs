using System;
using Isu.Entities;
using IsuExtra.Models;

namespace IsuExtra.Entities
{
    public class GroupStudyClass : StudyStream
    {
        internal GroupStudyClass(
            Subject subject,
            TimeStamp timeStamp,
            Group group,
            Teacher teacher,
            Room room)
            : base(timeStamp, teacher, room)
        {
            Subject = subject ?? throw new ArgumentNullException(
                nameof(subject),
                $"{nameof(subject)} can't be null!");

            Group = group ?? throw new ArgumentNullException(
                nameof(group),
                $"{nameof(group)} can't be null!");
        }

        public Group Group { get; }
        public Subject Subject { get; }
    }
}