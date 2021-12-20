using Core.Domain.Tools;

namespace Core.Domain.Entities.TaskChanges
{
    public class StateChange : JobTaskChange
    {
        public JobTaskState NewState { get; private init; }

        private StateChange()
        {
        }

        public StateChange(JobTaskState newState)
        {
            NewState = newState;
        }
    }
}