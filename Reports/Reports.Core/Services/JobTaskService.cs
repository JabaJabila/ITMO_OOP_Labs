using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.ServicesAbstractions;
using Core.Domain.Tools;

namespace Core.Services
{
    public class JobTaskService : IJobTaskService
    {
        public Task<List<JobTask>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> FindByCreationTime(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> FindByLastChangeTime(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<List<JobTask>> FindAssignedTasks(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<List<JobTask>> FindContributedTasks(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<List<JobTask>> FindAssignedToSubordinatesTasks(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<JobTask> CreateTask(string name, Employee assignedEmployee)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDescription(JobTask task, string description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeState(JobTask task, JobTaskState state)
        {
            throw new NotImplementedException();
        }

        public Task ChangeAssignedEmployee(JobTask task, Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task AddComment(JobTask task, Employee author, string message)
        {
            throw new NotImplementedException();
        }
    }
}