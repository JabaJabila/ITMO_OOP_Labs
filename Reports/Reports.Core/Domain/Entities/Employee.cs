using System;
using System.Collections.Generic;
using Core.Domain.Tools;

namespace Core.Domain.Entities
{
    public class Employee
    {
        private readonly List<Employee> _subordinates;
        private Employee _supervisor;
        
        public Guid Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<Employee> Subordinates => _subordinates;

        public Employee Supervisor
        {
            get => _supervisor;
            set
            {
                if (!CheckIfObeys(value))
                {
                    _supervisor.DeleteSubordinate(this);
                    _supervisor = value;
                }
                else
                    throw new ReportsAppException($"Impossible to set employee {value.Id} as supervisor for" +
                                                  $" {Id}. Employee {value.Id} is lower in hierarchy!");
            }
        }

        public Employee(string name, Employee supervisor = null, List<Employee> subordinates = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "Name is invalid");
            }

            Id = Guid.NewGuid();
            Name = name;
            _subordinates = new List<Employee>();
            Supervisor = supervisor;
            subordinates?.ForEach(AddSubordinate);
        }

        public void AddSubordinate(Employee subordinate)
        {
            if (subordinate == null)
                throw new ArgumentNullException(nameof(subordinate));

            subordinate.Supervisor = this;
            _subordinates.Add(subordinate);
        }
        
        public void DeleteSubordinate(Employee subordinate)
        {
            if (subordinate == null)
                throw new ArgumentNullException(nameof(subordinate));

            _subordinates.Remove(subordinate);
            subordinate._supervisor = null;
        }

        public void Delete()
        {
            _subordinates.ForEach(employee => employee._supervisor = Supervisor);
            Supervisor.DeleteSubordinate(this);
        }

        private bool CheckIfObeys(Employee employee)
        {
            if (employee == null)
                return false;

            bool result = false;
            foreach (Employee subordinate in employee.Subordinates)
                 result = subordinate == employee || subordinate.CheckIfObeys(employee);
            
            return result;
        }
    }
}