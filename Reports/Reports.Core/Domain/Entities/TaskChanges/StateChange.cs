using Core.Domain.Tools;

namespace Core.Domain.Entities.TaskChanges
{
    public class StateChange : JobTaskChange
    {
        public JobTaskState NewState { get; private init; }

        private StateChange()
        {
        }

        public StateChange(Employee author, JobTaskState newState)
            : base(author)
        {
            NewState = newState;
        }
    }
}