using System;

namespace Core.Domain.Entities.TaskChanges
{
    public abstract class JobTaskChange
    {
        public DateTime ChangeTime { get; private init; }
        public Guid Id { get; private init; }

        protected JobTaskChange()
        {
            ChangeTime = DateTime.Now;
        }
    }
}