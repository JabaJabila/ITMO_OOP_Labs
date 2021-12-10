using System;

namespace Domain.Entities
{
    public class TaskModel
    {
        public Guid Id { get; set; }

        public Employee AssignedEmployee { get; set; }
    }
}