using System;

namespace Backups.Entities
{
    public class Storage
    {
        public Storage(string fullName)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        }

        public string FullName { get; }
    }
}