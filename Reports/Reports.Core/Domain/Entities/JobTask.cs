using System;
using System.Collections.Generic;
using Core.Domain.Entities.Enums;
using Core.Domain.Entities.TaskChanges;

namespace Core.Domain.Entities
{
    public class JobTask
    {
        public Guid Id { get; private init; }
        public string Name { get; private init; }
        public DateTime CreationTime { get; private init; }
        public string Description { get; set; }
        public JobTaskState CurrentState { get; set; }
        public List<JobTaskChange> Changes { get; private init; }
        public Employee AssignedEmployee { get; set; }

        private JobTask()
        {
        }
        
        public JobTask(string name, Employee employee, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            AssignedEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            Changes = new List<JobTaskChange>();
            CurrentState = JobTaskState.Open;
            CreationTime = DateTime.Now;
        }
    }
}