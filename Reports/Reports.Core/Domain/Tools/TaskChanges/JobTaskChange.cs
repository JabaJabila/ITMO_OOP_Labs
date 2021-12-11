using System;

namespace Core.Domain.Tools.TaskChanges
{
    public abstract class JobTaskChange
    {
        public DateTime ChangeTime { get; }

        protected JobTaskChange()
        {
            ChangeTime = DateTime.Now;
        }
    }
}