using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.Enums;

namespace Core.Domain.ServicesAbstractions
{
    public interface IJobTaskService
    {
        Task<IReadOnlyCollection<JobTask>> GetAll();
        Task<JobTask> GetById(Guid id);
        Task<JobTask> FindByName(string name);
        Task<JobTask> FindByCreationTime(DateTime dateTime);
        Task<JobTask> FindByLastChangeTime(DateTime dateTime);
        Task<IReadOnlyCollection<JobTask>> FindAssignedTasks(Employee employee);
        Task<IReadOnlyCollection<JobTask>> FindContributedTasks(Employee employee);
        Task<IReadOnlyCollection<JobTask>> FindAssignedToSubordinatesTasks(Employee employee);
        Task<JobTask> CreateTask(string name, Employee assignedEmployee, string description);
        Task<JobTask> ChangeDescription(JobTask task, string description);
        Task<JobTask> ChangeState(JobTask task, JobTaskState state);
        Task<JobTask> ChangeAssignedEmployee(JobTask task, Employee employee);
        Task<JobTask> AddComment(JobTask task, Employee author, string message);
    }
}