using System;
using System.Collections.Generic;

namespace Core.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private init; }
        public string Name { get; set; }
        public Employee Supervisor { get; set; }
        public List<Employee> Subordinates { get; private init; }

        private Employee()
        {
        }

        public Employee(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Subordinates = new List<Employee>();
        }
    }
}