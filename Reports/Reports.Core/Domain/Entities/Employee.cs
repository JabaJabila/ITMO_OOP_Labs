using System;
using System.Collections.Generic;
using Core.Domain.Tools;

namespace Core.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private init; }
        public string Name { get; set; }

        public EmployeeStatus Status { get; set; }
        public Employee Supervisor { get; set; }
        public List<Employee> Subordinates { get; private init; }

        private Employee()
        {
        }

        public Employee(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Subordinates = new List<Employee>();
            Status = EmployeeStatus.Working;
        }
    }
}