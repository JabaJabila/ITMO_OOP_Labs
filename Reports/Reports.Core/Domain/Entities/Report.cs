using System;
using System.Collections.Generic;
using Core.Domain.Tools;

namespace Core.Domain.Entities
{
    public class Report
    {
        private readonly List<JobTask> _tasks;
        private string _description;
        
        public ReportType Type { get; }
        public DateTime CreationTime { get; }
        public Employee AssignedEmployee { get; }
        public ReportState State { get; set; }
        public IReadOnlyCollection<JobTask> Tasks => _tasks;

        public string Description
        {
            get => _description;
            set
            {
                if (State == ReportState.Finished)
                    throw new ReportsAppException("You can't make changes in finished report!");

                _description = value;
            }
        }

        public Report(Employee employee, ReportType type, string description = null, List<JobTask> tasks = null)
        {
            AssignedEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            Type = type;
            Description = description;
            CreationTime = DateTime.Now;
            _tasks = new List<JobTask>();

            tasks?.ForEach(task =>
            {
                if (task != null)
                    _tasks.Add(task);
            });
        }

        public void AddTask(JobTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            
            if (State == ReportState.Finished)
                throw new ReportsAppException("You can't make changes in finished report!");
            
            if (!_tasks.Contains(task))
                _tasks.Add(task);
        }

        public void DeleteTask(JobTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            
            if (State == ReportState.Finished)
                throw new ReportsAppException("You can't make changes in finished report!");

            _tasks.Remove(task);
        }
    }
}