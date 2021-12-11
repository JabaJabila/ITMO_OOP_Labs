using System;

namespace Core.Domain.Entities
{
    public class Task
    {
        public Guid Id { get; set; }

        public Employee AssignedEmployee { get; set; }
    }
}