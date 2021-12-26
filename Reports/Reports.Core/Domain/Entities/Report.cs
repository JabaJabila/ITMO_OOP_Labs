using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; private init; }
        public ReportType Type { get; private init; }
        public DateTime CreationTime { get; private init; }
        public Employee AssignedEmployee { get; private init; }
        public ReportState State { get; set; }
        public List<JobTask> Tasks { get; private init; }
        public string Description { get; set; }

        private Report()
        {
        }

        public Report(Employee employee, ReportType type, string description = null, List<JobTask> tasks = null)
        {
            AssignedEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            Type = type;
            Description = description;
            CreationTime = DateTime.Now;
            Tasks = new List<JobTask>();

            tasks?.ForEach(task =>
            {
                if (task != null)
                    Tasks.Add(task);
            });
        }
    }
}