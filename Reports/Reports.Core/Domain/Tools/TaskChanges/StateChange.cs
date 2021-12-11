namespace Core.Domain.Tools.TaskChanges
{
    public class StateChange : JobTaskChange
    {
        public JobTaskState NewState { get; }

        public StateChange(JobTaskState newState)
        {
            NewState = newState;
        }
    }
}