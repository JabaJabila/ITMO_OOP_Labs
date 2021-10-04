using System;
using Isu.Tools;

namespace Isu.DataTypes
{
    public class CourseNumber : IEquatable<CourseNumber>
    {
        private const int NumOfFirstCourse = 1;
        private const int NumOfLastCourse = 4;
        public CourseNumber(int number)
        {
            if (number < NumOfFirstCourse || number > NumOfLastCourse)
                throw new IsuException("CourseNumber must be in range [1..4]");
            Number = number;
        }

        public int Number { get; }

        public bool Equals(CourseNumber other)
        {
            return other != null
                   && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            return obj is CourseNumber courseNumber && Equals(courseNumber);
        }

        public override int GetHashCode()
        {
            return Number;
        }
    }
}