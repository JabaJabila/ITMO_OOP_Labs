using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Entities.TaskChanges;
using Core.Domain.ServicesAbstractions;
using Core.Domain.Tools;
using Core.RepositoryAbstractions;

namespace Core.Services
{
    public class JobTaskService : IJobTaskService
    {
        private readonly IJobTaskRepository _taskRepository;
        private readonly ITaskChangesRepository _changesRepository;

        public JobTaskService(IJobTaskRepository taskRepository, ITaskChangesRepository changesRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _changesRepository = changesRepository ?? throw new ArgumentNullException(nameof(changesRepository));
        }

        public async Task<IReadOnlyCollection<JobTask>> GetAll()
        {
            return await _taskRepository.GetAll();
        }

        public async Task<JobTask> GetById(Guid id)
        {
            return await _taskRepository.GetById(id);
        }

        public async Task<JobTask> FindByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return (await _taskRepository.GetAll()).FirstOrDefault(t =>
                t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<JobTask> FindByCreationTime(DateTime dateTime)
        {
            return (await _taskRepository.GetAll()).FirstOrDefault(t => t.CreationTime == dateTime);
        }

        public async Task<JobTask> FindByLastChangeTime(DateTime dateTime)
        {
            return (await _taskRepository.GetAll()).FirstOrDefault(
                t => t.Changes.LastOrDefault()?.ChangeTime == dateTime);
        }

        public async Task<IReadOnlyCollection<JobTask>> FindAssignedTasks(Employee employee)
        {
            return (await _taskRepository.GetAll())
                .Where(t => t.AssignedEmployee.Id == employee.Id)
                .ToList();
        }

        public async Task<IReadOnlyCollection<JobTask>> FindContributedTasks(Employee employee)
        {
            return (await _taskRepository.GetAll())
                .Where(t => t.Changes.Any(c => c.Author.Id == employee.Id))
                .ToList();
        }

        public async Task<IReadOnlyCollection<JobTask>> FindAssignedToSubordinatesTasks(Employee employee)
        {
            return (await _taskRepository.GetAll())
                .Where(t => t.AssignedEmployee.SupervisorId == employee.Id)
                .ToList();
        }

        public async Task<JobTask> CreateTask(string name, Employee assignedEmployee, string description)
        {
            var task = new JobTask(name, assignedEmployee, description);
            return await _taskRepository.Add(task);
        }

        public async Task<JobTask> ChangeDescription(JobTask task, string description)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            task.Description = description ?? throw new ArgumentNullException(nameof(description));
            var change = new DescriptionChange(task.AssignedEmployee, description);
            task.Changes.Add(change);
            await _changesRepository.Add(change);
            await _changesRepository.SaveChanges();
            await _taskRepository.SaveChanges();
            return task;
        }

        public async Task<JobTask> ChangeState(JobTask task, JobTaskState state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            task.CurrentState = state;
            var change = new StateChange(task.AssignedEmployee, state);
            task.Changes.Add(change);
            await _changesRepository.Add(change);
            await _changesRepository.SaveChanges();
            await _taskRepository.SaveChanges();
            return task;
        }

        public async Task<JobTask> ChangeAssignedEmployee(JobTask task, Employee employee)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            var change = new AssignedEmployeeChange(task.AssignedEmployee, employee);
            task.AssignedEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            task.Changes.Add(change);
            await _changesRepository.Add(change);
            await _changesRepository.SaveChanges();
            await _taskRepository.SaveChanges();
            return task;
        }

        public async Task<JobTask> AddComment(JobTask task, Employee author, string message)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (author == null) throw new ArgumentNullException(nameof(author));
            if (message == null) throw new ArgumentNullException(nameof(message));
            var comment = new CommentChange(author, message);
            task.Changes.Add(comment);
            await _changesRepository.Add(comment);
            await _changesRepository.SaveChanges();
            await _taskRepository.SaveChanges();
            return task;
        }
    }
}