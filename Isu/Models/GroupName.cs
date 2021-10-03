using System;
using Isu.DataTypes;
using Isu.Tools;

namespace Isu.Models
{
    public class GroupName : IEquatable<GroupName>
    {
        private const char BachelorNumber = '3';
        private const uint MinimalLengthOfEndOfGroupName = 2;
        private readonly string _endOfGroupName;
        private char _facultyLetter;

        public GroupName(
            char facultyLetter,
            CourseNumber courseNumber,
            string endOfGroupName,
            char degreeOfEducation = BachelorNumber)
        {
            if (endOfGroupName.Length < MinimalLengthOfEndOfGroupName)
                throw new IsuException($"Length of the endOfGroupName must be > {MinimalLengthOfEndOfGroupName}");

            FacultyLetter = facultyLetter;
            DegreeOfEducation = degreeOfEducation;

            CourseNumber = courseNumber ?? throw new ArgumentNullException(
                    nameof(courseNumber),
                    $"{nameof(courseNumber)} can't be null!");

            _endOfGroupName = endOfGroupName;
        }

        public char FacultyLetter
        {
            get => _facultyLetter;
            set
            {
                if (value < 'A' || value > 'Z')
                    throw new IsuException("Faculty letter must be in range [A-Z]");
                _facultyLetter = value;
            }
        }

        public CourseNumber CourseNumber { get; }
        public char DegreeOfEducation { get; }
        public string Name
            => FacultyLetter + DegreeOfEducation + CourseNumber.Number + _endOfGroupName;

        public bool Equals(GroupName other)
        {
            return other != null
                   && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is GroupName groupName && Equals(groupName);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}