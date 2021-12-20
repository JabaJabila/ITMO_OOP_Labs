using System;

namespace Core.Domain.Entities.TaskChanges
{
    public class AssignedEmployeeChange : JobTaskChange
    {
        public Employee NewAssignedEmployee { get; private init; }

        private AssignedEmployeeChange()
        {
        }
        
        public AssignedEmployeeChange(Employee newAssignedEmployee)
        {
            NewAssignedEmployee = newAssignedEmployee ?? throw new ArgumentNullException(nameof(newAssignedEmployee));
        }
    }
}