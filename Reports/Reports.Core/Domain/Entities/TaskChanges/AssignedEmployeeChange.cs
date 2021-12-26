using System;

namespace Core.Domain.Entities.TaskChanges
{
    public class AssignedEmployeeChange : JobTaskChange
    {
        public Employee NewAssignedEmployee { get; private init; }

        private AssignedEmployeeChange()
        {
        }
        
        public AssignedEmployeeChange(Employee previousEmployee, Employee newAssignedEmployee)
            : base(previousEmployee)
        {
            NewAssignedEmployee = newAssignedEmployee
                                  ?? throw new ArgumentNullException(nameof(newAssignedEmployee));
        }
    }
}