using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Tools;

namespace Core.Domain.ServicesAbstractions
{
    public interface IJobTaskService
    {
        Task<List<JobTask>> GetAll();
        Task<JobTask> GetById(Guid id);
        Task<JobTask> FindByName(string name);
        Task<JobTask> FindByCreationTime(DateTime dateTime);
        Task<JobTask> FindByLastChangeTime(DateTime dateTime);
        Task<List<JobTask>> FindAssignedTasks(Employee employee);
        Task<List<JobTask>> FindContributedTasks(Employee employee);
        Task<List<JobTask>> FindAssignedToSubordinatesTasks(Employee employee);
        Task<JobTask> CreateTask(string name, Employee assignedEmployee);
        Task ChangeDescription(JobTask task, string description);
        Task ChangeState(JobTask task, JobTaskState state);
        Task ChangeAssignedEmployee(JobTask task, Employee employee);
        Task AddComment(JobTask task, Employee author, string message);
    }
}