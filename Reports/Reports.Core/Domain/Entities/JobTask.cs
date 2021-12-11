using System;
using System.Collections.Generic;
using Core.Domain.Tools;
using Core.Domain.Tools.TaskChanges;

namespace Core.Domain.Entities
{
    public class JobTask
    {
        private readonly List<JobTaskChange> _changes;
        private Employee _assignedEmployee;
        private JobTaskState _state;
        
        public Guid Id { get; }
        public string Name { get; }
        public DateTime CreationTime { get; }

        public JobTaskState CurrentState
        {
            get => _state;
            set
            {
                if (CurrentState == value) return;
                _state = value; 
                CommitChange(new StateChange(value));
            }
        }
        public IReadOnlyCollection<JobTaskChange> Changes => _changes;

        public Employee AssignedEmployee
        {
            get => _assignedEmployee;
            set
            {
                if (_assignedEmployee == value) return;
                _assignedEmployee = value ?? throw new ArgumentNullException(nameof(value));
                CommitChange(new AssignedEmployeeChange(value));
            } 
        }
        
        public JobTask(string name, Employee employee)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AssignedEmployee = employee;
            _changes = new List<JobTaskChange>();
            CurrentState = JobTaskState.Open;
            CreationTime = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public void AddComment(CommentChange comment)
        {
            CommitChange(comment);
        }
        
        private void CommitChange(JobTaskChange change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));
            
            _changes.Add(change);
        }
    }
}