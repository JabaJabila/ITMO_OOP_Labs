using Isu.Tools;

namespace Isu.DataTypes
{
    public class CourseNumber
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
    }
}