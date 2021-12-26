using System;
using Core.Domain.Entities.Enums;

namespace Core.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private init; }
        public string Name { get; set; }

        public EmployeeStatus Status { get; set; }
        public Guid? SupervisorId { get; set; }

        private Employee()
        {
        }

        public Employee(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Status = EmployeeStatus.Working;
        }
    }
}