using System;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Models
{
    public class GroupName : IEquatable<GroupName>
    {
        private const string BachelorNumber = "3";
        private const uint MinimalLengthOfEndOfGroupName = 2;
        private readonly string _endOfGroupName;

        public GroupName(Faculty faculty, Course course, string endOfGroupName)
        {
            if (endOfGroupName.Length < MinimalLengthOfEndOfGroupName)
                throw new IsuException($"Length of the endOfGroupName must be > {MinimalLengthOfEndOfGroupName}");

            Faculty = faculty ?? throw new ArgumentNullException(
                    nameof(faculty),
                    $"{nameof(faculty)} can't be null!");

            Course = course ?? throw new ArgumentNullException(
                    nameof(course),
                    $"{nameof(course)} can't be null!");

            _endOfGroupName = endOfGroupName;
        }

        public Course Course { get; }
        public Faculty Faculty { get; }

        public string Name
            => Faculty.Letter + BachelorNumber + Course.CourseNumber.Number + _endOfGroupName;

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