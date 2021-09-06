using Isu.Tools;

namespace Isu.DataTypes
{
    public class CourseNumber
    {
        private uint _number;
        public CourseNumber(uint number)
        {
            if (number < 1 || number > 4)
                throw new IsuException("CourseNumber must be in range [1..4]");
            _number = number;
        }

        public uint Number
        {
            get => _number;
        }
    }
}