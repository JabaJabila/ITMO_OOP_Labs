using System;
using Isu.DataTypes;
using Isu.Tools;

namespace IsuExtra.Models
{
    public class TimeStamp
    {
        public TimeStamp(
            TimeSpan startTime,
            TimeSpan endTime,
            DayOfWeek day,
            DateTime dateOfStart,
            DateTime dateOfEnd,
            WeekAlternation weekAlternation = WeekAlternation.Both)
        {
            TimeSpan diffTime = endTime - startTime;
            if (diffTime.Duration() != diffTime)
                throw new IsuException("EndTime can't be earlier than StartTime!");
            if (dateOfEnd < dateOfStart)
                throw new IsuException("Date of End Time can't be earlier than Date of Start Time!");

            Day = day;
            StartTime = startTime;
            EndTime = endTime;
            DateOfStart = dateOfStart;
            DateOfEnd = dateOfEnd;
            WeekAlternation = weekAlternation;
        }

        public DayOfWeek Day { get; }
        public DateTime DateOfStart { get; }
        public DateTime DateOfEnd { get; }
        public WeekAlternation WeekAlternation { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public bool CheckIfIntersects(TimeStamp other)
        {
            if (DateOfEnd < other.DateOfStart || DateOfStart > other.DateOfEnd)
                return false;
            if ((WeekAlternation != other.WeekAlternation
                 && WeekAlternation != WeekAlternation.Both
                 && other.WeekAlternation != WeekAlternation.Both)
                || Day != other.Day)
                return false;
            return (StartTime <= other.EndTime && StartTime >= other.StartTime) ||
                   (EndTime >= other.StartTime && EndTime <= other.EndTime) ||
                   (StartTime <= other.StartTime && EndTime >= other.StartTime) ||
                   (StartTime >= other.StartTime && EndTime <= other.EndTime);
        }
    }
}