using System;

namespace Core.Domain.Entities.TaskChanges
{
    public abstract class JobTaskChange
    {
        public DateTime ChangeTime { get; private init; }
        public Employee Author { get; private init; }
        public Guid Id { get; private init; }

        protected JobTaskChange()
        {
        }
        
        protected JobTaskChange(Employee author)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            ChangeTime = DateTime.Now;
        }
    }
}