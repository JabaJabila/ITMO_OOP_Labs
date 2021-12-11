using System;
using Core.Domain.Entities;

namespace Core.Domain.Tools.TaskChanges
{
    public class AssignedEmployeeChange : JobTaskChange
    {
        public Employee NewAssignedEmployee { get; }

        public AssignedEmployeeChange(Employee newAssignedEmployee)
        {
            NewAssignedEmployee = newAssignedEmployee ?? throw new ArgumentNullException(nameof(newAssignedEmployee));
        }
    }
}