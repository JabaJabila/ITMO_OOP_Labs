using Isu.Tools;

namespace Isu.DataTypes
{
    public class CourseNumber
    {
        public CourseNumber(int number)
        {
            if (number < 1 || number > 4)
                throw new IsuException("CourseNumber must be in range [1..4]");
            Number = number;
        }

        public int Number { get; }
    }
}