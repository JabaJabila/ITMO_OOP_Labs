using System;

namespace Backups.Entities
{
    public class JobObject : IEquatable<JobObject>
    {
        public JobObject(string fullName)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        }

        public string FullName { get; }

        public bool Equals(JobObject other)
        {
            return other != null
                   && FullName.Equals(other.FullName);
        }

        public override bool Equals(object obj)
        {
            return obj is JobObject jobObject && Equals(jobObject);
        }

        public override int GetHashCode()
        {
            return FullName != null ? FullName.GetHashCode() : 0;
        }
    }
}